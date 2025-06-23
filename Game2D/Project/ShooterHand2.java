import greenfoot.*;

public class ShooterHand2 extends Actor {
    private int lifetime = 300;
    private int cooldown = 30;  // Увеличенный КД для веера
    private int shootCount = 0;
    private int maxShots = 5;   // Количество вееров
    private int frameCount = 0;
    private int spreadAngle = 30; // Угол разброса (+/- 30 градусов)
    private double koef = 1;
    
    public void set_coef (double koef){
        this.koef = koef;
    }

    public void act() {
        if (getWorld() == null) return;
        
        frameCount++;
        lifetime--;
        
        if (lifetime <= 0 || shootCount >= maxShots) {
            getWorld().removeObject(this);
            return;
        }

        if (frameCount % cooldown == 0) {
            shootFanAtHero();
            shootCount++;
        }
    }

  private void shootFanAtHero() {
    if (getWorld() == null) return;

    if (Shoot.getActiveCount() > 250 + (int)(koef * 50)) return;

    Hero hero = (Hero)getWorld().getObjects(Hero.class).get(0);
    if (hero == null) return;

    int deltaX = hero.getX() - getX();
    int deltaY = hero.getY() - getY();
    int baseAngle = (int)Math.toDegrees(Math.atan2(deltaY, deltaX));
    int randomSpread = 30 + Greenfoot.getRandomNumber(16); 

    int bulletsInFan = (int)(6*koef);
    for (int i = 0; i < bulletsInFan; i++) {
        int angle = baseAngle - randomSpread / 2 + (randomSpread * i) / (bulletsInFan - 1);

        if (Shoot.getActiveCount() >=  250 + (int)(koef * 50)) break;

        Shoot bullet = new Shoot(angle);
        getWorld().addObject(bullet, getX(), getY());
    }
}
}