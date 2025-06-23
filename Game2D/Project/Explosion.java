import greenfoot.*;

public class Explosion extends Actor {
    private int animationTimer = 0;
    private int animationDuration = 20; // Длительность анимации в кадрах

    public Explosion() {
        GreenfootImage img = new GreenfootImage("boom.png");
        removeWhiteBackground(img);
        setImage(img);
        img.scale(img.getWidth() / 2, img.getHeight() / 2);
    }

    public void act() {
        animationTimer++;
        if (animationTimer >= animationDuration) {
            if (getWorld() != null) {
                getWorld().removeObject(this);
            }
        }
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