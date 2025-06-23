import greenfoot.*;  // (World, Actor, GreenfootImage, Greenfoot and MouseInfo)

public class HealthBar extends Actor {
    private int health;
    private int maxHealth;

    public HealthBar(int maxHealth) {
        this.maxHealth = maxHealth;
        this.health = maxHealth;
        updateImage();
    }

    public void updateHealth(int newHealth) {
        this.health = newHealth;
        updateImage();
    }

    private void updateImage() {
        GreenfootImage img = new GreenfootImage(104, 16);
        img.setColor(Color.GRAY);
        img.fillRect(0, 0, 104, 16);
        img.setColor(Color.RED);
        int healthWidth = (int) ((double) health / maxHealth * 100);
        img.fillRect(2, 2, healthWidth, 10);
        setImage(img);
    }
}
