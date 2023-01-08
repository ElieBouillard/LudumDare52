using System;
using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using CMF;
using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    [SerializeField] private Transform _camera;
    [SerializeField] private ParticleSystem _muzzleFlashVFX;
    [SerializeField] private ParticleSystem _impactVFX;
    [SerializeField] private Bullet _bullet;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private float _reloadTime;
    [SerializeField] private float _damage;
    
    private NetworkManager _networkManager;
    private ushort _playerId;
    private bool _isLocal;
    private AdvancedWalkerController _controller;
    
    private Ressource _lastRessource;

    public bool CanShoot = true;

    private int _bulletAmount = 6;
    private bool _isInReload;
    
    private void Start()
    {
        _networkManager = NetworkManager.Instance;
        _playerId = GetComponent<PlayerIdentity>().GetId;
        _isLocal = _networkManager.LocalPlayer.GetId == _playerId;
        _controller = GetComponent<AdvancedWalkerController>();
    }

    private void Update()
    {
        if (!_isLocal) return;

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!_isInReload && _bulletAmount < 6)
            {
                StartCoroutine(Reload());
            }
        }
        
        if(!CanShoot || _isInReload) return;
        
        OutlineRessources();

        if (_controller.IsSprinting || _controller.CurrentControllerState != AdvancedWalkerController.ControllerState.Grounded) return;
        
        if(_bulletAmount <= 0) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(_camera.transform.position + _camera.transform.forward * 5f, _camera.transform.forward, out RaycastHit hit, Mathf.Infinity))
            {
                _networkManager.ClientMessages.SendShoot(hit.point, true, hit.normal);
                
                Shoot(hit.point, hit.normal);
                
                if (hit.collider.TryGetComponent(out Ressource ressource))
                {
                    ressource.TakeDamage(_playerId);
                }

                if (hit.collider.TryGetComponent(out PlayerGameIdentity player))
                {
                    player.GetComponent<PlayerDistantHealth>().TakeDamage(_damage);
                    _networkManager.ClientMessages.SendDamage(player.GetId, _damage);
                }
            }
            else
            {
                _networkManager.ClientMessages.SendShoot(Camera.main.transform.position + Camera.main.transform.forward * 250f, false, Vector3.zero);
                Shoot(Camera.main.transform.position + Camera.main.transform.forward * 250f, null);
            }

            _bulletAmount--;
            BulletsPanel.Instance.UpdateBulletsAmount(_bulletAmount);
            
            if (_bulletAmount <= 0)
            {
                StartCoroutine(Reload());
            }
        }
    }

    private void OutlineRessources()
    {
        if (!Physics.Raycast(_camera.transform.position, _camera.transform.forward, out RaycastHit hit, Mathf.Infinity)) return;
        
        if (hit.collider.TryGetComponent(out Ressource ressource))
        {
            if (_lastRessource != ressource)
            {
                ressource.EnableOutline(true);
                _lastRessource = ressource;
            }
        }
        else
        {
            if (_lastRessource != null)
            {
                _lastRessource.EnableOutline(false);
                _lastRessource = null;
            }
        }
    }
    
    public void Shoot(Vector3 targetPos, Vector3? normal)
    {
        _muzzleFlashVFX.Play();
        Bullet bullet = Instantiate(_bullet, _spawnPoint.position, _spawnPoint.rotation);
        bullet.Initialize(targetPos);

        if (normal == null) return;
        ParticleSystem impactInstance = Instantiate(_impactVFX, targetPos, Quaternion.identity);
        impactInstance.transform.forward = normal.Value;
        impactInstance.Play();
        
        Destroy(impactInstance.gameObject, 2f);
    }

    private IEnumerator Reload()
    {
        _isInReload = true;
        
        BulletsPanel.Instance.StartReloadImage(_reloadTime);
        
        yield return new WaitForSeconds(_reloadTime);

        BulletsPanel.Instance.UpdateBulletsAmount(6);
        
        _bulletAmount = 6;
        
        _isInReload = false;
    }
}
