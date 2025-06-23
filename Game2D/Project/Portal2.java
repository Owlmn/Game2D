import greenfoot.*;  // (World, Actor, GreenfootImage, Greenfoot and MouseInfo)

/**
 * Класс Portal — портал с прозрачным фоном.
 */
public class Portal2 extends Actor
{
    public Portal2() {
        GreenfootImage image = new GreenfootImage("portal2.jpg");
        removeWhiteBackground(image);
        setImage(image);
    }

    public void act()
    {
        // Действия портала
    }

    
    private void removeWhiteBackground(GreenfootImage image) {
        for (int x = 0; x < image.getWidth(); x++) {
            for (int y = 0; y < image.getHeight(); y++) {
                Color color = image.getColorAt(x, y);
                if (color.getRed() > 240 && color.getGreen() > 240 && color.getBlue() > 240) {
                    image.setColorAt(x, y, new Color(255, 255, 255, 0)); // Прозрачный пиксель
                }
            }
        }
    }
}
