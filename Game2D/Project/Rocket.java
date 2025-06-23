import greenfoot.*;

public class Rocket extends Actor {
    private double dx, dy;
    private int speed = 5;
    private double koef=1;
    private int damage = (int)(100 * koef);

    public Rocket(int angle, double coef) {
        this.koef = coef;
        GreenfootImage img = new GreenfootImage("rocket.png");
        removeWhiteBackground(img);
        setImage(img);
        img.scale(img.getWidth() / 2, img.getHeight() / 2);
        setRotation(angle);
        
        dx = Math.cos(Math.toRadians(angle)) * speed;
        dy = Math.sin(Math.toRadians(angle)) * speed;
    }

    public void act() {
        if (isAtEdge() || checkCollision()) {
            createExplosion();
            if (getWorld() != null) {
                getWorld().removeObject(this);
            }
            return;
        }
        
        moveRocket();
    }

    private void moveRocket() {
        setLocation((int)(getX() + dx), (int)(getY() + dy));
    }

    private boolean checkCollision() {
        if (getWorld() == null) return false;

        // Столкновение со стенами
        if (getOneIntersectingObject(Wall_gorizont.class) != null ||
            getOneIntersectingObject(Wall_vertical.class) != null ||
            getOneIntersectingObject(Cube.class) != null) {
            return true;
        }

        // Столкновение с героем
        Hero hero = (Hero) getOneIntersectingObject(Hero.class);
        if (hero != null) {
            hero.takeDamage(damage);
            return true;
        }

        return false;
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