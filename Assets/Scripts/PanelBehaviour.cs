using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PanelBehaviour : MonoBehaviour
{
    [SerializeField] private Vector2 _panelSize;
    [SerializeField] private float _moveDuration;
    [SerializeField] private float _combineDuration;
    [SerializeField] private Vector3 _popSize;
    [SerializeField] private Color[] _colors;
    [SerializeField] private Color[] _fontColors;
    [SerializeField] private bool _logNotation;
    private Vector2Int _boardSize;
    private int _number;
    private Vector2Int _positionIndex;
    private bool _canCombine;
    private TextMesh _textMesh;
    private SpriteRenderer _renderer;
    private Vector3 _initialScale;
    private Dictionary<int, Color> _colorDictionary;
    private Dictionary<int, Color> _fontColorDictionary;

    private Vector3 BoardOrigin => new Vector3(-_panelSize.x / 2 * (_boardSize.x - 1), -_panelSize.y / 2 * (_boardSize.y - 1));
    private Vector3 IndexToPosition(Vector2Int positionIndex) => BoardOrigin + new Vector3(positionIndex.x * _panelSize.x, positionIndex.y * _panelSize.y);
    public void Initialize(Vector2Int boardSize, Vector2Int positionIndex, int number) {
        _boardSize = boardSize;
        _positionIndex = positionIndex;
        _number = number;
        _canCombine = true;

        transform.position = IndexToPosition(_positionIndex);
        _textMesh.text = _logNotation ? ((int)Mathf.Log(_number, 2)).ToString() : _number.ToString();
        _textMesh.color = _textMesh.color = _fontColorDictionary.ContainsKey(_number) ? _fontColorDictionary[_number] : _fontColors[^1];
        _renderer.color = _colorDictionary.ContainsKey(_number) ? _colorDictionary[_number] : _colors[^1];
        gameObject.SetActive(true);
    }

    public Tween Move(Vector2Int newPositionIndex) => transform.DOMove(IndexToPosition(newPositionIndex), _moveDuration).OnStart(() => { _positionIndex = newPositionIndex; });
    public bool CanCombineWith(PanelBehaviour panel) => _canCombine && _number == panel._number;
    public Tween CombineWith(PanelBehaviour panel) => 
        DOTween.Sequence().AppendInterval(_moveDuration - _combineDuration)
                          .AppendCallback(() => { 
                            _textMesh.text = _logNotation ? ((int)Mathf.Log(_number, 2)).ToString() : _number.ToString();
                            _textMesh.color = _fontColorDictionary.ContainsKey(_number) ? _fontColorDictionary[_number] : _fontColors[^1];
                            _renderer.color = _colorDictionary.ContainsKey(_number) ? _colorDictionary[_number] : _colors[^1];
                          })
                          .Append(panel._renderer.DOFade(0f,_combineDuration))
                          .Join(transform.DOScale(_popSize, _combineDuration / 2).OnComplete(() => {
                              transform.DOScale(_initialScale, _combineDuration / 2);
                          })).OnStart(() => { _number += panel._number; }); 
    public void SetCanCombine(bool value) => _canCombine = value;

    private void Awake() {
        gameObject.SetActive(false);
        _textMesh = GetComponentInChildren<TextMesh>();
        _renderer = GetComponent<SpriteRenderer>();
        _initialScale = transform.localScale;
        _colorDictionary = new Dictionary<int, Color>();
        _fontColorDictionary = new Dictionary<int, Color>();

        int n = 2;
        foreach (var item in _colors) {
            _colorDictionary.Add(n, item);
            n *= 2;
        }

        n = 2;
        foreach (var item in _fontColors) {
            _fontColorDictionary.Add(n, item);
            n *= 2;
        }
    }
}
