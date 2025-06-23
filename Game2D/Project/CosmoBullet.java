import greenfoot.*;

public class CosmoBullet extends Actor {
    private double dx, dy;
    private int speed = 6; 
    private double koef=1;
    private int damage = (int)(20 * koef); 

    public CosmoBullet(int angle, double coef) {
        this.koef = coef;
        GreenfootImage img = new GreenfootImage("cosmo_bullet.png");
        removeWhiteBackground(img);
        img.scale(img.getWidth() / 2, img.getHeight() / 2);
        setImage(img);
        setRotation(angle);
        
        dx = Math.cos(Math.toRadians(angle)) * speed;
        dy = Math.sin(Math.toRadians(angle)) * speed;
    }

    public void act() {
        if (isAtEdge() || checkCollision()) {
            getWorld().removeObject(this);
            return;
        }
        
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