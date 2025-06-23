import greenfoot.*;  // (World, Actor, GreenfootImage, Greenfoot and MouseInfo)

public class Laser extends Actor
{
    private double dx,dy;
    private int speed=8;
    
    public Laser(double angle){
        setRotation((int)Math.toDegrees(angle)+90);
        dx=Math.cos(angle)*speed;
        dy=Math.sin(angle)*speed;
        
        setImage("laser.png");
    }
    public void act()
    {
        if (isAtEdge() || checkCollision()){
            World world=getWorld();
            if (world!=null) world.removeObject(this);
            return;
        }
        
        checkCollision();
        if (!isInWorld()) return;
        MoveLaser();
        
    }
    
    private boolean isInWorld(){
        return getWorld() != null;
    }
    
    private void MoveLaser(){
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

        // Столкновение с зомби
        zombi zombie = (zombi) getOneIntersectingObject(zombi.class);
        if (zombie != null) {
            zombie.takeDamage(100); // ← Используем метод убийства
            return true;
        }

        // Столкновение с обычным врагом
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
            cosmo.takeDamage(75); 
            MyWorld world = (MyWorld) getWorld();
            if (world != null) {
                world.incrementKillCount(); 
            }
            return true;
        }
        
        ShotgunEnemy shot = (ShotgunEnemy) getOneIntersectingObject(ShotgunEnemy.class);
        if (shot != null) {
            shot.takeDamage(75); 
            MyWorld world = (MyWorld) getWorld();
            if (world != null) {
                world.incrementKillCount(); 
            }
            return true;
        }
        
        Boss boss = (Boss) getOneIntersectingObject(Boss.class);
          if (boss != null) {
          boss.takeDamage(18); // Наносим урон боссу (можно настроить значение)
          return true; // Пуля исчезает после попадания
        }

        return false;
    }
}
