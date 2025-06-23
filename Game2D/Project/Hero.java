import greenfoot.*;

public class Hero extends Actor {
    private GreenfootImage rifle=new GreenfootImage("hero.png");
    private GreenfootImage shotgun=new GreenfootImage("hero_shotgun.png");
    private GreenfootImage lasergun=new GreenfootImage("hero_lasergun.png");
    
    private int direction;
    private boolean alive;
    
    private int MAX_HEALTH = 3000;
    public int health=MAX_HEALTH;

    private boolean isBuffed = false;
    private int buffTimer = 0;
    private final int buffDuration = 300;

    private int reloadTimer = 0;

    private int shootTimer = 0;

    private WeaponType currentWeapon = WeaponType.RIFLE;
    int weaponLevel=1;

    // Оружие и боезапас
    private int laserAmmo = 10, maxLaserAmmo = 10;
    private int shotgunAmmo = 5, maxShotgunAmmo = 5;
    private int rifleAmmo = 30, maxRifleAmmo = 30;

    private final int laserCooldown = 10;
    private final int shotgunCooldown = 120;
    private final int rifleCooldown = 80;

    public Hero(int startX, int startY, int startDirection) {
        setImageWithoutWhite("hero.png");

        this.direction = startDirection;
        this.alive = true;
        this.health = MAX_HEALTH;

        setRotation(direction * 90);
    }

    private void setImageWithoutWhite(String filename) {
        GreenfootImage image = new GreenfootImage(filename);
        for (int x = 0; x < image.getWidth(); x++) {
            for (int y = 0; y < image.getHeight(); y++) {
                if (image.getColorAt(x, y).equals(Color.WHITE)) {
                    image.setColorAt(x, y, new Color(0, 0, 0, 0));
                }
            }
        }
        setImage(image);
    }

    public void move(int dx, int dy) {
        if (!alive) return;

        int newX = getX() + dx;
        int newY = getY() + dy;

        if (canMove(dx, dy)) {
            setLocation(newX, newY);
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

    public void act() {
        if (!alive) return;

        handleBuff();
        turnTowardsMouse();
        
        switchWeapon();
        shoot();
        checkWeaponPickup();
        reload();
    }

    private void handleBuff() {
        if (isBuffed) {
            buffTimer++;
            if (buffTimer >= buffDuration) {
                isBuffed = false;
                buffTimer = 0;
            }
        }
    }

    private void switchWeapon() {
        if (Greenfoot.isKeyDown("3") && weaponLevel>=3){
            currentWeapon = WeaponType.LASERGUN;
            setImage(lasergun);
        }
        if (Greenfoot.isKeyDown("2") && weaponLevel>=2) {
            currentWeapon = WeaponType.SHOTGUN;
            setImage(shotgun);
        }
        if (Greenfoot.isKeyDown("1") && weaponLevel>=1) {
            currentWeapon = WeaponType.RIFLE;
            setImage(rifle);
        }
    }
    
    private void checkWeaponPickup() {
        WeaponPickup pickup = (WeaponPickup) getOneIntersectingObject(WeaponPickup.class);
        if (pickup != null) {
            currentWeapon = pickup.getWeaponType();
            switch (currentWeapon) {
            case LASERGUN:
                setImage(lasergun);
                break;
            case SHOTGUN:
                setImage(shotgun);
                break;
            case RIFLE:
                setImage(rifle);
                break;
        }
            weaponLevel++;
            getWorld().removeObject(pickup);
        }
    }

    public void shoot() {
        shootTimer++;
        if (!Greenfoot.mousePressed(null)) return;

        MouseInfo mouse = Greenfoot.getMouseInfo();
        if (mouse == null) return;

        int dx = mouse.getX() - getX();
        int dy = mouse.getY() - getY();
        double angle = Math.atan2(dy, dx);

        switch (currentWeapon) {
            case LASERGUN:
                if (shootTimer >= laserCooldown && laserAmmo > 0) {
                    fireLaser(angle);
                    laserAmmo--;
                    shootTimer = 0;
                }
                break;
            case SHOTGUN:
                if (shootTimer >= shotgunCooldown && shotgunAmmo > 0) {
                    for (int i = -1; i <= 1; i++) {
                        fireBullet(angle + i * 0.2);
                    }
                    shotgunAmmo--;
                    shootTimer = 0;
                }
                break;
            case RIFLE:
                if (shootTimer >= rifleCooldown && rifleAmmo > 0) {
                    fireBullet(angle);
                    rifleAmmo--;
                    shootTimer = 0;
                }
                break;
        }

        updateAmmoLabel();
    }

    private void fireBullet(double angle) {
        Bullet bullet = new Bullet(getX()+10, getY()+10, angle);
        getWorld().addObject(bullet, getX()+10, getY()+10);
    }
    
    private void fireLaser(double angle) {
        Laser laser = new Laser(angle);
        getWorld().addObject(laser, getX()+5, getY()+14);
    }

    public void reload() {
        reloadTimer++;
        if (reloadTimer >= (isBuffed ? 60 : 120)) {
            reloadTimer = 0;
            switch (currentWeapon) {
                case LASERGUN:
                    if (laserAmmo < maxLaserAmmo) laserAmmo++;
                    break;
                case SHOTGUN:
                    if (shotgunAmmo < maxShotgunAmmo) shotgunAmmo++;
                    break;
                case RIFLE:
                    if (rifleAmmo < maxRifleAmmo) rifleAmmo++;
                    break;
            }
            updateAmmoLabel();
        }
    }

    private void updateAmmoLabel() {
        MyWorld world = (MyWorld) getWorld();
        if (world != null) {
            int ammo = switch (currentWeapon) {
                case LASERGUN -> laserAmmo;
                case SHOTGUN -> shotgunAmmo;
                case RIFLE -> rifleAmmo;
            };
            world.updateAmmoDisplay(ammo);
        }
    }

    private void updateHealthLabel() {
        MyWorld world = (MyWorld) getWorld();
        if (world != null) {
            world.getHealthBar().updateHealth(health);
        }
    }

    public void takeDamage(int damage) {
        if (!alive) return;

        health -= damage;
        if (health < 0) health = 0;
        updateHealthLabel();

        if (health <= 0) die();
    }

    private void die() {
        alive = false;
        getWorld().addObject(new Died() , getWorld().getWidth()/2,getWorld().getHeight()/2);
        getWorld().removeObject(this);
        Greenfoot.stop();
    }
    
    public boolean isAlive(){
        return alive;
    }
    
    public int getMaxHealth(){
        return MAX_HEALTH;
    }

    public void applyBuff() {
        if (!isBuffed) {
            isBuffed = true;
            buffTimer = 0;
        }
    }

    private void turnTowardsMouse() {
        MouseInfo mouse = Greenfoot.getMouseInfo();
        if (mouse != null) {
            int dx = mouse.getX() - getX();
            int dy = mouse.getY() - getY();
            double angle = Math.atan2(dy, dx);
            setRotation((int) Math.toDegrees(angle));
        }
    }

    public boolean checkPortalCollision() {
        return getOneIntersectingObject(Portal.class) != null;
    }

    public boolean checkPortal2Collision() {
        return getOneIntersectingObject(Portal2.class) != null;
    }
    
    public boolean checkPortal3Collision() {
        return getOneIntersectingObject(Portal3.class) != null;
    }
}
