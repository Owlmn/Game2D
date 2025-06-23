import greenfoot.*;
import java.util.List;
import java.util.Random;

public class Bullet extends Actor {
    private double dx,dy;
    private static final int SPEED = 30;


    public Bullet(int startX, int startY, double angle) {
        GreenfootImage image = new GreenfootImage("bullet.png");
        image.scale(image.getWidth() * 3 / 2, image.getHeight() * 3 / 2);
        removeWhiteBackground(image);
        setImage(image);
        setRotation((int)Math.toDegrees(angle));
        dx=Math.cos(angle)*SPEED;
        dy=Math.sin(angle)*SPEED;
    }

    public void act() {
        MoveRound();

        if (checkCollision()) {
            if (getWorld() != null) {
                getWorld().removeObject(this);
            }
        }
    }
    
    private void MoveRound(){
        setLocation((int)(getX()+dx),(int)(getY()+dy));
    }

    private boolean checkCollision() {
        if (getWorld() == null) return false;

        // Столкновение со стенами
        if (getOneIntersectingObject(Wall_gorizont.class) != null ||
            getOneIntersectingObject(Wall_vertical.class) != null ||
            getOneIntersectingObject(Cube.class) != null) {
            return true;
        }

       
        zombi zombie = (zombi) getOneIntersectingObject(zombi.class);
        if (zombie != null) {
            zombie.takeDamage(100); 
            return true;
        }

    
        Enemy_beast enemy = (Enemy_beast) getOneIntersectingObject(Enemy_beast.class);
        if (enemy != null) {
            MyWorld world = (MyWorld) getWorld();
            if (world != null) {
                world.incrementKillCount();
                world.removeObject(enemy);
            }
            return true;
        }
        
        RocketEnemy rocketEnemy = (RocketEnemy) getOneIntersectingObject(RocketEnemy.class);
        if (rocketEnemy != null) {
            rocketEnemy.takeDamage(75);
            MyWorld world = (MyWorld) getWorld();
            if (world != null) {
                world.incrementKillCount(); 
            }
            return true;
        }
        
        CosmoEnemy cosmo = (CosmoEnemy) getOneIntersectingObject(CosmoEnemy.class);
        if (cosmo != null) {
            cosmo.takeDamage(85); 
            MyWorld world = (MyWorld) getWorld();
            if (world != null) {
                world.incrementKillCount(); 
            }
            return true;
        }
        
        ShotgunEnemy shot = (ShotgunEnemy) getOneIntersectingObject(ShotgunEnemy.class);
        if (shot != null) {
            shot.takeDamage(90); 
            MyWorld world = (MyWorld) getWorld();
            if (world != null) {
                world.incrementKillCount(); 
            }
            return true;
        }
        
        Boss boss = (Boss) getOneIntersectingObject(Boss.class);
          if (boss != null) {
          boss.takeDamage(30); 
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
