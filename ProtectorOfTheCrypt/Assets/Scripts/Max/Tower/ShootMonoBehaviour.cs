using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ShootMonoBehaviour : MonoBehaviour
{
    public TowerScriptableObject tower;
    private bool paused;

    private Transform projectileSpawnPoint;


    public void Activate(TowerScriptableObject Tower)
    {
        projectileSpawnPoint = transform.Find("ProjectileSpawnpoint");
        tower = Tower;
        tower = tower.Clone() as TowerScriptableObject;
        if(tower.Name == "ExplosiveTower") IsExplosive();
    }
    public void OnEnable()
    {
        GameManager.instance.OnGamePaused += UpdateGamePaused;
    }
    private void UpdateGamePaused(bool isPaused)
    {
        paused = isPaused;
    }
    private void OnDisable()
    {
        GameManager.instance.OnGamePaused -= UpdateGamePaused;
    }
    private void Update()
    {
        if(paused)
            return;
        
        tower.Shoot(projectileSpawnPoint.position);
    }

    private void IsExplosive()
    {
        new ImpactTypeModifier()
        {
            Amount = tower.ImpactTypeOverride
        }.Apply(tower);

        SetExplosiveDamage();
    }

    public void SetExplosiveDamage()
    {
        tower.BulletImpactEffects = new ICollisionHandler[]
        {
            new Explode(
                tower.AOERange,
                new AnimationCurve(new Keyframe[] { new Keyframe(0, 1), new Keyframe(1, 1) }), // No damage fall off atm, lowering the y value of the second keyframe will add damage fall off
                tower.AOEDamage,
                20
            )
        };
    }
}
