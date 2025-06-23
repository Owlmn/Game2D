import greenfoot.*;

public class zombi extends Actor {
    double koef=1;
    private int health = (int)(120*koef);
    private int speed = (int)(2*koef);
    private Hero hero;
    
    private int shootCooldown = 0;
    private final int shootDelay = (int)(90/koef); // 1.5 секунды
    
    private final int DETECTION_RANGE = 400;   // Начинают видеть героя
    private final int SHOOTING_RANGE = 250;    // Начинают стрелять

    public zombi(Hero hero,double coef) {
        this.hero = hero;
        koef=coef;

        GreenfootImage img = getImage();
        img.scale(img.getWidth() / 10, img.getHeight() / 10);
        setImage(img);
    }

    public void act() {
        if (hero != null && hero.isAlive()) {
            moveTowardsHero();
            shootAtHero();
            checkForHeroCollision();
        }
    }

    private void moveTowardsHero() {
        int dx = hero.getX() - getX();
        int dy = hero.getY() - getY();

        int moveX = Integer.compare(dx, 0) * speed;
        int moveY = Integer.compare(dy, 0) * speed;

        if (canMove(moveX, 0)) {
            setLocation(getX() + moveX, getY());
        }
        if (canMove(0, moveY)) {
            setLocation(getX(), getY() + moveY);
        }
    }

    private boolean canMove(int dx, int dy) {
        return getOneObjectAtOffset(dx, dy+30, Wall_gorizont.class) == null && getOneObjectAtOffset(dx+30, dy, Wall_gorizont.class) == null &&
                getOneObjectAtOffset(dx, dy-30, Wall_gorizont.class) == null && getOneObjectAtOffset(dx-30, dy, Wall_gorizont.class) == null &&
               getOneObjectAtOffset(dx+30, dy, Wall_vertical.class) == null && getOneObjectAtOffset(dx, dy+30, Wall_vertical.class) == null &&
               getOneObjectAtOffset(dx-30, dy, Wall_vertical.class) == null && getOneObjectAtOffset(dx, dy-30, Wall_vertical.class) == null &&
               getOneObjectAtOffset(dx+30, dy, Cube.class) == null && getOneObjectAtOffset(dx-30, dy, Cube.class) == null &&
               getOneObjectAtOffset(dx, dy+30, Cube.class) == null && getOneObjectAtOffset(dx, dy-30, Cube.class) == null;
    }

    private void shootAtHero() {
        shootCooldown++;
        if (shootCooldown >= shootDelay) {
            shootCooldown = 0;

            int dx = hero.getX() - getX();
            int dy = hero.getY() - getY();

       
            int angle;
            if (Math.abs(dx) > Math.abs(dy)) {
                angle = dx > 0 ? 0 : 180;
            } else {
                angle = dy > 0 ? 90 : 270;
            }

            setRotation(angle);

            Poison poison = new Poison();
            poison.setRotation(angle);

         
            int offsetX = (int)(20 * Math.cos(Math.toRadians(angle)));
            int offsetY = (int)(20 * Math.sin(Math.toRadians(angle)));

            getWorld().addObject(poison, getX() + offsetX, getY() + offsetY);
        }
    }

    private void checkForHeroCollision() {
        if (isTouching(Hero.class)) {
            Hero h = (Hero) getOneIntersectingObject(Hero.class);
            if (h != null) {
                h.takeDamage(5);
            }
        }
    }

    public void takeDamage(int damage) {
    health -= damage;
    if (health <= 0) {
        MyWorld world = (MyWorld) getWorld();
        world.incrementKillCount(); 
        hero.applyBuff(); // Применяем бафф после убийства зомби
        getWorld().removeObject(this);
    }
}

}
