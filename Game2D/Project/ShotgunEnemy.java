import greenfoot.*;

public class ShotgunEnemy extends Actor {
    private double koef = 1;
    private Hero hero;
    private int shootCooldown = 0;
    private final int shootDelay = (int)(180 / koef); // Большая задержка между выстрелами
    private int health = (int)(150 * koef);
    

    private final int DETECTION_RANGE = 250; 
    private final int SHOOTING_RANGE = 200;  
    private int speed = 2; 

    private GreenfootImage image;
    private boolean isActive = false;

    public ShotgunEnemy(int startX, int startY, int startDirection, double coef, Hero hero) {
        this.hero = hero;
        this.koef = coef;
        

        image = new GreenfootImage("shouterenemy.png");
        removeWhiteBackground(image);
        image.scale(image.getWidth()/2, image.getHeight()/2);
        setImage(image);
        
        setRotation(startDirection * 90);
        setLocation(startX, startY);
    }

    public void act() {
        if (hero == null || !hero.isAlive()) {
            hero = getHero();
            if (hero == null) return;
        }
        
        double distance = calculateDistanceToHero();
        updateBehaviorState(distance);
            
        if (isActive) {
            turnTowardsHero();
            moveTowardsHero();
            tryShoot();
        }
    }
    
    private double calculateDistanceToHero() {
        return Math.hypot(hero.getX() - getX(), hero.getY() - getY());
    }
    
    private void updateBehaviorState(double distance) {
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
            dx = (int)((dx/length) * speed-0.5);
            dy = (int)((dy/length) * speed-0.5);
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
    
    private void tryShoot() {
        shootCooldown++;
        if (shootCooldown >= shootDelay && calculateDistanceToHero() <= SHOOTING_RANGE) {
            shoot();
            shootCooldown = 0;
        }
    }

    private void shoot() {
        double angle = Math.toRadians(getRotation());
        // Выстрел тремя дробинками с небольшим разбросом
        for (int i = -1; i <= 1; i++) {
            ShotgunPellet pellet = new ShotgunPellet(getRotation() + i*15, koef); // ±15 градусов разброс
            int offsetX = (int)(30 * Math.cos(angle));
            int offsetY = (int)(30 * Math.sin(angle));
            getWorld().addObject(pellet, getX() + offsetX, getY() + offsetY);
        }
    }

    public void takeDamage(int damage) {
        health -= damage;
        if (health <= 0) {
            destroy();
        }
    }
    
    private void destroy() {
        MyWorld world = (MyWorld) getWorld();
        if (world != null) {
            world.incrementKillCount();
            createExplosion();
            getWorld().removeObject(this);
        }
    }
    
    private void createExplosion() {
        getWorld().addObject(new Explosion(), getX(), getY());
    }
    
    private Hero getHero() {
        if (getWorld() == null) return null;
        return getWorld().getObjects(Hero.class).stream().findFirst().orElse(null);
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