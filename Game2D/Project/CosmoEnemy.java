import greenfoot.*;

public class CosmoEnemy extends Actor {
    private double koef = 1;
    private Hero hero;
    private int shootCooldown = 0;
    private final int shootDelay = (int)(200 / koef); 
    private int health = (int)(120 * koef);
    

    private final int DETECTION_RANGE = 320;  
    private final int SHOOTING_RANGE = 250;  
    private int speed = 2; 
    
   
    private GreenfootImage[] walkFrames;
    private int currentFrame = 0;
    private int animationDelay = 5;
    private int animationTimer = 0;
    private boolean isActive = false;
    private boolean isMoving = false;
    private boolean isInShootingRange = false;

    public CosmoEnemy(Hero hero, double coef) {
        this.hero = hero;
        this.koef = coef;
        
        
        walkFrames = new GreenfootImage[8];
        for (int i = 0; i < 8; i++) {
            walkFrames[i] = new GreenfootImage("cosmo_walk_" + (i+1) + ".png");
            removeWhiteBackground(walkFrames[i]);
            walkFrames[i].scale((int)(walkFrames[i].getWidth()/1.5), (int)(walkFrames[i].getHeight()/1.5));
        }
        
        setImage(walkFrames[0]);
    }

    public void act() {
        if (hero == null || !hero.isAlive()) return;
            
        double distance = calculateDistanceToHero();
        updateBehaviorState(distance);
            
        if (isActive) {
            turnTowardsHero();
            moveTowardsHero();
            animateWalk();
            
            if (isInShootingRange) {
                shootAtHero();
            }
        }
    }
    
    private void updateBehaviorState(double distance) {
      
        if (!isActive && distance <= DETECTION_RANGE) {
            isActive = true;
        }
        
        
        isInShootingRange = (distance <= SHOOTING_RANGE);
    }
    
    private double calculateDistanceToHero() {
        return Math.hypot(hero.getX() - getX(), hero.getY() - getY());
    }
    
    private void checkActivation(double distance) {
        if (!isActive && distance <= DETECTION_RANGE) {
            isActive = true;
        }
    }
    
    private void turnTowardsHero() {
        turnTowards(hero.getX(), hero.getY());
    }
    
    private void moveTowardsHero() {
        int dx = hero.getX() - getX();
        int dy = hero.getY() - getY();
        
      
        double length = Math.sqrt(dx*dx + dy*dy);
        if (length > 0) {
            dx = (int)((dx/length) * speed);
            dy = (int)((dy/length) * speed);
        }
        
        if (canMove(dx, 0)) {
            setLocation(getX() + dx, getY());
            isMoving = true;
        }
        if (canMove(0, dy)) {
            setLocation(getX(), getY() + dy);
            isMoving = true;
        }
    }
    
    private boolean canMove(int dx, int dy) {
        return getOneObjectAtOffset(dx, dy+10, Wall_gorizont.class) == null && getOneObjectAtOffset(dx+10, dy, Wall_gorizont.class) == null &&
                getOneObjectAtOffset(dx, dy-10, Wall_gorizont.class) == null && getOneObjectAtOffset(dx-10, dy, Wall_gorizont.class) == null &&
               getOneObjectAtOffset(dx+10, dy, Wall_vertical.class) == null && getOneObjectAtOffset(dx, dy+10, Wall_vertical.class) == null &&
               getOneObjectAtOffset(dx-10, dy, Wall_vertical.class) == null && getOneObjectAtOffset(dx, dy-10, Wall_vertical.class) == null &&
               getOneObjectAtOffset(dx+10, dy, Cube.class) == null && getOneObjectAtOffset(dx-10, dy, Cube.class) == null &&
               getOneObjectAtOffset(dx, dy+10, Cube.class) == null && getOneObjectAtOffset(dx, dy-10, Cube.class) == null;
    }
    
    private void animateWalk() {
        if (isMoving) {
            animationTimer++;
            if (animationTimer >= animationDelay) {
                animationTimer = 0;
                currentFrame = (currentFrame + 1) % walkFrames.length;
                setImage(walkFrames[currentFrame]);
            }
        }
        isMoving = false;
    }
    
    private void shootAtHero() {
        shootCooldown++;
        if (shootCooldown >= shootDelay) {
            shootCooldown = 0;
            
            CosmoBullet bullet = new CosmoBullet(getRotation(), koef);
            int offsetX = (int)(40 * Math.cos(Math.toRadians(getRotation())));
            int offsetY = (int)(40 * Math.sin(Math.toRadians(getRotation())));
            getWorld().addObject(bullet, getX() + offsetX, getY() + offsetY);
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