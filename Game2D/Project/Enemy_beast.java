import greenfoot.*;

public class Enemy_beast extends Actor {
    double koef=1;
    private GreenfootImage[] walkFrames;
    private GreenfootImage[] idleFrames;
    private int animationIndex = 0;
    private int animationSpeed = 7;
    private int frameCounter = 0;
    private boolean moving = false;
    private boolean alive = true;
    private int damage = (int)(50*koef);

    private int attackCooldown = (int)(60/koef); 
    private int attackTimer = 0;

    public Enemy_beast(int startX, int startY, int startDirection,double coef) {
        walkFrames = new GreenfootImage[4];
        idleFrames = new GreenfootImage[2];
        koef=coef;
        for (int i = 0; i < 4; i++) {
            walkFrames[i] = new GreenfootImage("enemy_go_" + (i + 1) + ".png");
            removeWhiteBackground(walkFrames[i]);
        }

        idleFrames[0] = new GreenfootImage("enemy1.png");
        idleFrames[1] = new GreenfootImage("enemy_breath.png");
        removeWhiteBackground(idleFrames[0]);
        removeWhiteBackground(idleFrames[1]);

        setImage(idleFrames[0]);
        setRotation(startDirection * 90);
        setLocation(startX, startY);
    }

    public void act() {
        if (attackTimer > 0) attackTimer--;

        followHero();
        animate();
        checkCollisionWithHero();
    }

    private void followHero() {
        Hero hero = getHero();
        if (hero == null || !alive) return;

        int heroX = hero.getX();
        int heroY = hero.getY();
        int myX = getX();
        int myY = getY();

        double distance = Math.hypot(heroX - myX, heroY - myY);

        if (distance < 320) {
            int dx = Integer.compare(heroX, myX);
            int dy = Integer.compare(heroY, myY);

            if (dx != 0 || dy != 0) {
                move((int)(dx * (koef+0.5)), (int)(dy * (koef+0.5)));
                updateRotation(dx, dy);
            } else {
                moving = false;
            }
        } else {
            moving = false;
        }
    }

    public void move(int dx, int dy) {
        int newX = getX() + dx;
        int newY = getY() + dy;

        if (canMove(dx, dy)) {
            setLocation(newX, newY);
            moving = true;
        } else {
            moving = false;
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

    private void checkCollisionWithHero() {
        Hero hero = (Hero) getOneIntersectingObject(Hero.class);
        if (hero != null && alive && attackTimer <= 0) {
            hero.takeDamage(damage);
            attackTimer = attackCooldown;
        }
    }

    private void animate() {
        frameCounter++;
        if (frameCounter % animationSpeed == 0) {
            GreenfootImage frame = moving
                ? walkFrames[animationIndex % walkFrames.length]
                : idleFrames[animationIndex % idleFrames.length];

            animationIndex++;
            setImage(frame);
        }
    }

    private void updateRotation(int dx, int dy) {
        if (dx > 0) setRotation(270);  // вправо
        else if (dx < 0) setRotation(90);  // влево
        else if (dy > 0) setRotation(0);   // вниз
        else if (dy < 0) setRotation(180); // вверх
    }

    private Hero getHero() {
        if (getWorld() == null) return null;
        java.util.List<Hero> heroes = getWorld().getObjects(Hero.class);
        return heroes.isEmpty() ? null : heroes.get(0);
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
