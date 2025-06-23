import greenfoot.*;

public class Shoot extends Actor {
    private static GreenfootImage bulletImage; // Статическое изображение для всех пуль
    private double dx, dy;
    private static int activeCount = 0;
    private int lifetime = 300; // Уменьшенное время жизни
    
    static {
        // Инициализация изображения один раз при загрузке класса
        try {
            bulletImage = new GreenfootImage("shoot.png"); // Лучше использовать PNG с прозрачностью
            bulletImage.scale(20, 20); // Оптимальный размер
        } catch (Exception e) {
            bulletImage = new GreenfootImage(10, 10);
            bulletImage.setColor(Color.RED);
            bulletImage.fillOval(0, 0, 10, 10);
        }
    }

    public Shoot(int angle) {
        setImage(bulletImage);
        setRotation(angle);
        double rad = Math.toRadians(angle);
        dx = Math.cos(rad) * 4;
        dy = Math.sin(rad) * 4;
        activeCount++;
    }

    public void act() {
        if (--lifetime <= 0) {
            removeSelf();
            return;
        }
        
        setLocation((int)(getX() + dx), (int)(getY() + dy));
        
        if (isAtEdge()) {
            removeSelf();
            return;
        }
        
        checkCollision();
    }

    private void checkCollision() {
        Hero hero = (Hero)getOneIntersectingObject(Hero.class);
        if (hero != null) {
            hero.takeDamage(25);
            removeSelf();
        }
    }

    private void removeSelf() {
        if (getWorld() != null) {
            getWorld().removeObject(this);
            activeCount--;
        }
    }

    public static int getActiveCount() {
        return activeCount;
    }
    private int spreadAngle = 40; // Больший разброс для второй руки

    private void shootFanAtHero() {
        if (getWorld() == null || Shoot.getActiveCount() > 30) return;

        Hero hero = (Hero)getWorld().getObjects(Hero.class).get(0);
        if (hero == null) return;

        int deltaX = hero.getX() - getX();
        int deltaY = hero.getY() - getY();
        int baseAngle = (int)Math.toDegrees(Math.atan2(deltaY, deltaX));

        int bulletsInFan = 7; // Другой количество пуль для разнообразия
        for (int i = 0; i < bulletsInFan; i++) {
            int angle = baseAngle - spreadAngle/2 + (spreadAngle * i)/(bulletsInFan-1);
            
            Shoot bullet = new Shoot(angle);
            getWorld().addObject(bullet, getX(), getY());
            Greenfoot.delay(1);
        }
    }
}