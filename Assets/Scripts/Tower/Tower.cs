using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField] private TowerData data;
    [SerializeField] private Animator animator;
    private bool isHighlighted = false;
    private CircleCollider2D _circleCollider;
    private SpriteRenderer _spriteRenderer;
    private List<Enemy> _enemiesInRange;
    private ObjectPooler _projectilePool;
    private bool moving = false;
    private Platform target;

    private float _shootTimer;

    private void OnEnable()
    {
        Enemy.OnEnemyDestroyed += HandleEnemyDestroyed;
    }

    private void OnDisable()
    {
        Enemy.OnEnemyDestroyed -= HandleEnemyDestroyed;
    }

    private void Start()
    {
        _circleCollider = GetComponent<CircleCollider2D>();
        _circleCollider.radius = data.range;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _enemiesInRange = new List<Enemy>();
        _projectilePool = GetComponent<ObjectPooler>();
        _shootTimer = data.shootInterval;
    }

    private void Update()
    {
        if (Time.timeScale == 0f) return;
        if(moving)
        {
            if(transform.position != target.transform.position + new Vector3(0f, 0.5f, 0f))
            {
                //unlock menu
                transform.position = Vector3.MoveTowards(transform.position, target.transform.position + new Vector3(0f, 0.5f, 0f), 1 * Time.deltaTime);
            }
            else
            {
                moving = false;
                animator.SetBool("isFlying", false);
                target.ToggleMenuLock();
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, data.range);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            _enemiesInRange.Add(enemy);
            animator.SetBool("isAttacking", true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (_enemiesInRange.Contains(enemy))
            {
                _enemiesInRange.Remove(enemy);
            }
        }
    }

    private void Shoot()
    {
        _enemiesInRange.RemoveAll(enemy => enemy == null || !enemy.gameObject.activeInHierarchy);

        if (_enemiesInRange.Count > 0)
        {
            GameObject projectile = _projectilePool.GetPooledObject();
            projectile.transform.position = transform.position;
            projectile.SetActive(true);
            Vector2 _shootDirection = (_enemiesInRange[0].transform.position - transform.position).normalized;
            projectile.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(_shootDirection.y, _shootDirection.x) * Mathf.Rad2Deg);;
            projectile.GetComponent<Projectile>().Shoot(data, _shootDirection);
            SoundManager.Instance?.PlaySFX(data.shootSound);
        }
        else
        {
            animator.SetBool("isAttacking", false);
        }
    }

    private void HandleEnemyDestroyed(Enemy enemy)
    {
        _enemiesInRange.Remove(enemy);
    }

    //Toggles the color of the tower. Used when the tower is left-clicked for moving
    public void ToggleTowerHighlight()
    {
        Debug.Log(data.name);

        isHighlighted = !isHighlighted;
        _spriteRenderer.color = isHighlighted ? Color.red : Color.white;
    }

    public bool isTowerHighlighted()
    {
        return isHighlighted;
    }

    public void moveTo(Platform target)
    {
        //Lock platform menu
        target.ToggleMenuLock();
        animator.SetBool("isAttacking", false);
        animator.SetBool("isFlying", true);
        this.target = target;
        moving = true;
    }

    public TowerData getData()
    {
        return data;
    }
}
