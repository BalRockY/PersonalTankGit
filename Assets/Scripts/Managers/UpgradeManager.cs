using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{  

    


    // UPGRADES

    // Vehicle Movement 

    public void UpgradeMaxSpeed()
    {
        PlayerManager.Instance.tankConRef.maxSpeed += 1;
    }

    public void UpgradeTurnSpeed()
    {

    }

    // Weaponry

    public void UpgradeBulletCount()
    {
        PlayerManager.Instance.gunConRef.shotVolleyCount += 1;
    }

    public void UpgradeWeaponTurnSpeed()
    {
        PlayerManager.Instance.gunConRef.shotVolleyCount += 1;
    }

    public void UpgradeWeaponShootSpeed()
    {
        PlayerManager.Instance.gunConRef.shotVolleyCount += 1;
    }

    // Health Upgrades

    public void UpgradeMaxHP()
    {

    }

    public void OneTimeHP()
    {

    }






}
