using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TMPro;
using UnityEngine;

public class TemplateTrap : TimelineTickable, IPlayerCharacterEffect
{
    [TableMatrix] [OdinSerialize] private bool[,] Template = new bool[11, 11];
    [SerializeField] private int Cooldown = 2;
    [SerializeField] private GameObject TemplateCell;

    private int _templateCenterIndex = 5;
    private int _currentCooldown;
    private bool _effectTick;
    private readonly Dictionary<Vector3Int, GameObject> _templateMap = new();

    protected override void Awake()
    {
        base.Awake();

        _currentCooldown = Cooldown;
        
        for (int i = 0; i < Template.GetLength(0); i++)
        {
            for (int j = 0; j < Template.GetLength(1); j++)
            {
                if (!Template[i,j])
                    continue;

                var relativePos = new Vector3Int(i - _templateCenterIndex, j - _templateCenterIndex, 0);
                var pos = GridPosition + relativePos;
                var template = Instantiate(TemplateCell, transform);
                template.transform.position = Grid.CellToWorld(pos);
                _templateMap.Add(pos, template);
            }
        }

        UpdateTemplate();
    }

    public override void Tick(float tickDuration)
    {
        _effectTick = false;
        _currentCooldown--;
        if (_currentCooldown <= 0)
        {
            _currentCooldown = Cooldown;
            _effectTick = true;
            StartCoroutine(EffectAnimation(tickDuration));
        }
        else
        {
            StartCoroutine(CooldownAnimation(tickDuration));
        }
    }

    public void ApplyEffect(PlayerCharacter character)
    {
        character.Death();
    }

    public List<Vector3Int> EffectTiles()
    {
        return !_effectTick ? new List<Vector3Int>() : _templateMap.Keys.ToList();
    }

    private IEnumerator CooldownAnimation(float tickDuration)
    {
        float passedTime = 0f;
        while (tickDuration > passedTime)
        {
            passedTime += Time.deltaTime;

            if (passedTime > tickDuration / 2f)
            {
                UpdateTemplate();
            }
            
            yield return null;
        }
    }

    private IEnumerator EffectAnimation(float tickDuration)
    {
        foreach (var templateCell in _templateMap.Values)
        {
            templateCell.GetComponentInChildren<SpriteRenderer>().color = Color.red;
        }
        
        yield return new WaitForSeconds(tickDuration);
        
        UpdateTemplate();
    }

    private void UpdateTemplate()
    {
        foreach (var templateCell in _templateMap.Values)
        {
            templateCell.GetComponentInChildren<SpriteRenderer>().color = new Color(1f, 0f, 0f, 0.2f);
            templateCell.GetComponentInChildren<TextMeshPro>().text = _currentCooldown.ToString();
        }
    }

    private void OnDrawGizmos()
    {
        GridController grid = GetComponentInParent<GridController>();
        if (grid == null)
        {
            return;
        }
        
        Vector3Int currentPos = grid.WorldToCell(transform.position);
        List<Vector3Int> templateTiles = new ();
        for (int i = 0; i < Template.GetLength(0); i++)
        {
            for (int j = 0; j < Template.GetLength(1); j++)
            {
                if (!Template[i,j])
                    continue;

                var relativePos = new Vector3Int(i - _templateCenterIndex, j - _templateCenterIndex, 0);
                templateTiles.Add(currentPos + relativePos);
            }
        }
        
        for (var i = 0; i < templateTiles.Count; i++)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
            Gizmos.DrawCube(grid.CellToWorld(templateTiles[i]), new Vector3(1f, 1f, 0.1f));
        }
    }
}
