import greenfoot.*;

public class RocketEnemy extends Actor {
    private double koef = 1;
    private Hero hero;
    private int shootCooldown = 0;
    private final int shootDelay = (int)(130 / koef);
    private int health = (int)(150 * koef);
    
    //  дистанции поведения
    private final int DETECTION_RANGE = 400;   // Начинают видеть героя
    private final int SHOOTING_RANGE = 250;    // Начинают стрелять
    private final int MIN_DISTANCE = 200;      // Перестают приближаться
    private final int FORGET_RANGE = 450;      // Забывают героя
    private int speed = (int)(2 * koef);

    
    private GreenfootImage[] shootFrames;
    private int currentFrame = 0;
    private int animationDelay = 10; 
    private int animationTimer = 0;
    private boolean isAnimating = false;
    private boolean isActive = false;
    private boolean isInShootingRange = false;

    public RocketEnemy(Hero hero, double coef) {
        this.hero = hero;
        this.koef = coef;
        

        shootFrames = new GreenfootImage[3];
        shootFrames[0] = new GreenfootImage("launcher_1.png"); 
        shootFrames[1] = new GreenfootImage("launcher_2.png"); 
        shootFrames[2] = new GreenfootImage("launcher_3.png"); 
        
        shootFrames[0].scale(shootFrames[0].getWidth() / 2, shootFrames[0].getHeight() / 2);
        shootFrames[1].scale(shootFrames[1].getWidth() / 2, shootFrames[1].getHeight() / 2);
        shootFrames[2].scale(shootFrames[2].getWidth() / 2, shootFrames[2].getHeight() / 2);
        
        for (GreenfootImage frame : shootFrames) {
            removeWhiteBackground(frame);
        }
        
        setImage(shootFrames[0]); 
    }

    public void act() {
        if (hero == null || !hero.isAlive()) return;
            
        double distance = calculateDistanceToHero();
        updateBehaviorState(distance);
            
        if (isActive) {
            turnTowardsHero();
            
            if (distance > MIN_DISTANCE) {
                moveTowardsHero(distance);
            }
            
            if (isInShootingRange && !isAnimating) {
                shootRocket();
            } else if (isAnimating) {
                animateShoot();
            }
        }
    }
    
    private double calculateDistanceToHero() {
        return Math.hypot(hero.getX() - getX(), hero.getY() - getY());
    }
    
    private void updateBehaviorState(double distance) {
        // Активация/деактивация
        if (!isActive && distance <= DETECTION_RANGE) {
            isActive = true;
        } 
        else if (isActive && distance > FORGET_RANGE) {
            resetBehavior();
        }
        
     
        isInShootingRange = (distance <= SHOOTING_RANGE);
    }
    
    private void resetBehavior() {
        isActive = false;
        isAnimating = false;
        isInShootingRange = false;
        setImage(shootFrames[0]);
    }
    
    private void moveTowardsHero(double distance) {
        int dx = hero.getX() - getX();
        int dy = hero.getY() - getY();
        
     
        double length = Math.sqrt(dx*dx + dy*dy);
        if (length > 0) {
            dx = (int)((dx/length) * speed);
            dy = (int)((dy/length) * speed);
        }
        
        if (canMove(dx, 0)) setLocation(getX() + dx, getY());
        if (canMove(0, dy)) setLocation(getX(), getY() + dy);
    }
    
    private boolean canMove(int dx, int dy) {
        return getOneObjectAtOffset(dx, dy+10, Wall_gorizont.class) == null && getOneObjectAtOffset(dx+10, dy, Wall_gorizont.class) == null &&
                getOneObjectAtOffset(dx, dy-10, Wall_gorizont.class) == null && getOneObjectAtOffset(dx-10, dy, Wall_gorizont.class) == null &&
               getOneObjectAtOffset(dx+10, dy, Wall_vertical.class) == null && getOneObjectAtOffset(dx, dy+10, Wall_vertical.class) == null &&
               getOneObjectAtOffset(dx-10, dy, Wall_vertical.class) == null && getOneObjectAtOffset(dx, dy-10, Wall_vertical.class) == null &&
               getOneObjectAtOffset(dx+10, dy, Cube.class) == null && getOneObjectAtOffset(dx-10, dy, Cube.class) == null &&
               getOneObjectAtOffset(dx, dy+10, Cube.class) == null && getOneObjectAtOffset(dx, dy-10, Cube.class) == null;
    }
    
    private void turnTowardsHero() {
        if (hero != null) {
            turnTowards(hero.getX(), hero.getY());
        }
    }

    private void animateShoot() {
        animationTimer++;
        if (animationTimer >= animationDelay) {
            animationTimer = 0;
            currentFrame++;
            
         
            if (currentFrame < shootFrames.length) {
                setImage(shootFrames[currentFrame]);
                
                // Спавним ракету на последнем кадре
                if (currentFrame == 2) {
                    spawnRocket();
                }
            } else {
         
                currentFrame = 0;
                isAnimating = false;
                setImage(shootFrames[0]);
            }
        }
    }

    private void spawnRocket() {
        Rocket rocket = new Rocket(getRotation(), koef);
        int offsetX = (int)(30 * Math.cos(Math.toRadians(getRotation())));
        int offsetY = (int)(30 * Math.sin(Math.toRadians(getRotation())));
        getWorld().addObject(rocket, getX() + offsetX, getY() + offsetY);
    }

    private void shootRocket() {
        shootCooldown++;
        if (shootCooldown >= shootDelay) {
            shootCooldown = 0;
            isAnimating = true; 
            currentFrame = 0;
            animationTimer = 0;
        }
    }


    public void takeDamage(int damage) {
        health -= damage;
        if (health <= 0) {
            MyWorld world = (MyWorld) getWorld();
            if (world != null) {
                world.incrementKillCount();
                createExplosion();
                getWorld().removeObject(this);
            }
        }
    }

    private void createExplosion() {
        Explosion explosion = new Explosion();
        getWorld().addObject(explosion, getX(), getY());
    }

    private void removeWhiteBackground(GreenfootImage image) {
        for (int x = 0; x < image.getWidth(); x++) {
            for (int y = 0; y < image.getHeight(); y++) {
                if (image.getColorAt(x, y).equals(Color.WHITE)) {
                    image.setColorAt(x, y, new Color(0, 0, 0, 0));
                }
            }
        }
    }
}