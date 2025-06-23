import greenfoot.*;

public class Label extends Actor {
    private GreenfootImage image;
    private String text;
    private int size;

    public Label(String text, int size) {
        this.text = text;
        this.size = size;
        updateImage();
    }

    private void updateImage() {
        Font font = new Font("Arial", true, false, size);
        image = new GreenfootImage(text, size, Color.WHITE, new Color(0, 0, 0, 0));
        image.setFont(font);
        setImage(image);
    }

    public void setValue(String newText) {
        this.text = newText;
        updateImage();
    }
}
