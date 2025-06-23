import greenfoot.*;

public class WeaponPickup extends Actor {
    private WeaponType weaponType;

    public WeaponPickup(WeaponType weaponType) {
        this.weaponType = weaponType;
        setImage(getImageForWeapon(weaponType));
    }

    private GreenfootImage getImageForWeapon(WeaponType type) {
        switch (type) {
            case LASERGUN:
                return new GreenfootImage("lasergun.png");
            case SHOTGUN:
                return new GreenfootImage("shotgun.png");
            case RIFLE:
                //return new GreenfootImage("rifle.png");
            default:
                return new GreenfootImage(20, 20); // заглушка
        }
    }

    public WeaponType getWeaponType() {
        return weaponType;
    }
}