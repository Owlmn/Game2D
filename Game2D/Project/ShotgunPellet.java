import greenfoot.*;

public class ShotgunPellet extends Actor {
    private double dx, dy;
    private int speed = 15; 
    private double koef=1;
    private int damage = (int)(60 * koef); // Меньший урон чем у обычных пуль
    private int traveledDistance = 0;
    private final int MAX_DISTANCE = 150; // Короткая дистанция

    public ShotgunPellet(int angle, double coef) {
        this.koef = coef;
        GreenfootImage img = new GreenfootImage("shotgun_pellet.png");
        removeWhiteBackground(img);
        img.scale(img.getWidth()/2, img.getHeight()/2);
        setImage(img);
        setRotation(angle);
        
        double radAngle = Math.toRadians(angle);
        dx = Math.cos(radAngle) * speed;
        dy = Math.sin(radAngle) * speed;
    }

    public void act() {
        movePellet();
        traveledDistance += speed;
        
        if (traveledDistance >= MAX_DISTANCE || checkCollision()) {
            if (getWorld() != null) {
                getWorld().removeObject(this);
            }
        }
    }
    
    private void movePellet() {
        setLocation((int)(getX() + dx), (int)(getY() + dy));
    }

    private boolean checkCollision() {
        if (getWorld() == null) return false;

        // Столкновение с препятствиями
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