using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using TMPro;

public class PanelManager : MonoBehaviour
{
    [SerializeField] private PanelBehaviour _panel;
    [SerializeField] private int _initialPanelCount;
    [SerializeField] private bool _generateNewPanel;
    [SerializeField] private float _probabilityOfGenerate4;

    private Vector2Int _boardSize;
    private PanelBehaviour[,] _panelBehaviours;
    private bool _canInput;
    private Tween _moveTween;

    private void Shuffle<T>(IList<T> list) {
        for (int i = 0; i < list.Count; i++) {
            int randomIndex = Random.Range(0, list.Count);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }

    public void Initialize(Vector2Int boardSize) {

        _canInput = false;
        _moveTween = null;
        _boardSize = boardSize;

        var randomArray = new int[_boardSize.x * _boardSize.y];

        for (int i = 0; i < _initialPanelCount && i < randomArray.Length; i++) randomArray[i] = Random.value <= _probabilityOfGenerate4 ? 4 : 2;
        Shuffle(randomArray);

        _panelBehaviours = new PanelBehaviour[_boardSize.x, _boardSize.y];

        for (int i = 0; i < _boardSize.x; i++) {
            for (int j = 0; j < _boardSize.y; j++) {
                var number = randomArray[i + j * _boardSize.x];
                if (number > 0) {
                    var panel = Instantiate(_panel, transform);
                    panel.Initialize(_boardSize, new Vector2Int(i, j), number);
                    _panelBehaviours[i, j] = panel;
                }
            }
        }

        _canInput = true;
    }

    public void Initialize(Vector2Int boardSize, int[,] board) {

        _canInput = false;
        _moveTween = null;
        _boardSize = boardSize;

        _panelBehaviours = new PanelBehaviour[_boardSize.x, _boardSize.y];

        for (int i = 0; i < _boardSize.x; i++) {
            for (int j = 0; j < _boardSize.y; j++) {
                var number = (int)Mathf.Pow(2, board[_boardSize.y - 1 - j, i]);
                if (number > 1) {
                    var panel = Instantiate(_panel, transform);
                    panel.Initialize(_boardSize, new Vector2Int(i, j), number);
                    _panelBehaviours[i, j] = panel;
                }
            }
        }

         _canInput = true;
    }

    public void Destroy() {
        if (_panelBehaviours is null) return;

        for (int i = 0; i < _boardSize.x; i++) {
            for (int j = 0; j < _boardSize.y; j++) {
                if (_panelBehaviours[i, j] != null) Destroy(_panelBehaviours[i, j].gameObject);
            }
        }
    }

    private void GenerateNewPanel() {
        int empty = 0;

        for (int i = 0; i < _boardSize.x; i++) {
            for (int j = 0; j < _boardSize.y; j++) {
                if (_panelBehaviours[i, j] == null) empty++;
            }
        }

        if (empty == 0) return;

        int current = 0;
        int random = Random.Range(0, empty);

        for (int i = 0; i < _boardSize.x; i++) {
            for (int j = 0; j < _boardSize.y; j++) {
                if (_panelBehaviours[i, j] == null) {
                    if (current == random) {
                        var panel = Instantiate(_panel, transform);
                        panel.Initialize(_boardSize, new Vector2Int(i, j), Random.value <= _probabilityOfGenerate4 ? 4 : 2);
                        _panelBehaviours[i, j] = panel;
                    }
                    current++;
                }
            }
        }
    }

    private Tween MoveDown() {
        var current = new int[_boardSize.x];
        var topPanel = new PanelBehaviour[_boardSize.x];
        var sequence = DOTween.Sequence();
        var isChanged = false;

        for (int j = 0; j < _boardSize.y; j++) {
            for (int i = 0; i < _boardSize.x; i++) {
                if (_panelBehaviours[i, j] != null) {

                    var combine = topPanel[i] != null && topPanel[i].CanCombineWith(_panelBehaviours[i, j]);

                    if (combine) current[i]--;
                    if (j != current[i]) isChanged = true;
                    sequence.Join(_panelBehaviours[i, j].Move(new Vector2Int(i, current[i])));

                    if (combine) {
                        var behaviour = _panelBehaviours[i, j];
                        sequence.Join(topPanel[i].CombineWith(behaviour))
                                .AppendCallback(() => Destroy(behaviour.gameObject));
                        topPanel[i].SetCanCombine(false);
                        _panelBehaviours[i, j] = null;
                    }
                    else {
                        (_panelBehaviours[i, j], _panelBehaviours[i, current[i]]) = (_panelBehaviours[i, current[i]], _panelBehaviours[i, j]);
                    }
                    
                    topPanel[i] = _panelBehaviours[i, current[i]];
                    current[i]++;
                } 
            }
        }

        return isChanged ? sequence : null;
    }

    private Tween MoveUp() {
        var current = new int[_boardSize.x].Select(x => _boardSize.y - 1).ToArray();
        var topPanel = new PanelBehaviour[_boardSize.x];
        var sequence = DOTween.Sequence();
        var isChanged = false;

        for (int j = _boardSize.y - 1; j >= 0; j--) {
            for (int i = 0; i < _boardSize.x; i++) {
                if (_panelBehaviours[i, j] != null) {

                    var combine = topPanel[i] != null && topPanel[i].CanCombineWith(_panelBehaviours[i, j]);

                    if (combine) current[i]++;
                    if (j != current[i]) isChanged = true;
                    sequence.Join(_panelBehaviours[i, j].Move(new Vector2Int(i, current[i])));

                    if (combine) {
                        var behaviour = _panelBehaviours[i, j];
                        sequence.Join(topPanel[i].CombineWith(behaviour))
                                .AppendCallback(() => Destroy(behaviour.gameObject));
                        topPanel[i].SetCanCombine(false);
                        _panelBehaviours[i, j] = null;
                    }
                    else {
                        (_panelBehaviours[i, j], _panelBehaviours[i, current[i]]) = (_panelBehaviours[i, current[i]], _panelBehaviours[i, j]);
                    }
                    
                    topPanel[i] = _panelBehaviours[i, current[i]];
                    current[i]--;
                } 
            }
        }

        return isChanged ? sequence : null;
    }

    private Tween MoveLeft() {
        var current = new int[_boardSize.y];
        var topPanel = new PanelBehaviour[_boardSize.y];
        var sequence = DOTween.Sequence();
        var isChanged = false;

        for (int i = 0; i < _boardSize.x; i++) {
            for (int j = 0; j < _boardSize.y; j++) {
                if (_panelBehaviours[i, j] != null) {

                    var combine = topPanel[j] != null && topPanel[j].CanCombineWith(_panelBehaviours[i, j]);

                    if (combine) current[j]--;
                    if (i != current[j]) isChanged = true;
                    sequence.Join(_panelBehaviours[i, j].Move(new Vector2Int(current[j], j)));

                    if (combine) {
                        var behaviour = _panelBehaviours[i, j];
                        sequence.Join(topPanel[j].CombineWith(behaviour))
                                .AppendCallback(() => Destroy(behaviour.gameObject));
                        topPanel[j].SetCanCombine(false);
                        _panelBehaviours[i, j] = null;
                    }
                    else {
                        (_panelBehaviours[i, j], _panelBehaviours[current[j], j]) = (_panelBehaviours[current[j], j], _panelBehaviours[i, j]);
                    }
                    
                    topPanel[j] = _panelBehaviours[current[j], j];
                    current[j]++;
                } 
            }
        }

        return isChanged ? sequence : null;
    }

    private Tween MoveRight() {
        var current = new int[_boardSize.y].Select(x => _boardSize.x - 1).ToArray();
        var topPanel = new PanelBehaviour[_boardSize.y];
        var sequence = DOTween.Sequence();
        var isChanged = false;

        for (int i = _boardSize.x - 1; i >= 0; i--) {
            for (int j = 0; j < _boardSize.y; j++) {
                if (_panelBehaviours[i, j] != null) {

                    var combine = topPanel[j] != null && topPanel[j].CanCombineWith(_panelBehaviours[i, j]);

                    if (combine) current[j]++;
                    if (i != current[j]) isChanged = true;
                    sequence.Join(_panelBehaviours[i, j].Move(new Vector2Int(current[j], j)));

                    if (combine) {
                        var behaviour = _panelBehaviours[i, j];
                        sequence.Join(topPanel[j].CombineWith(behaviour))
                                .AppendCallback(() => Destroy(behaviour.gameObject));
                        topPanel[j].SetCanCombine(false);
                        _panelBehaviours[i, j] = null;
                    }
                    else {
                        (_panelBehaviours[i, j], _panelBehaviours[current[j], j]) = (_panelBehaviours[current[j], j], _panelBehaviours[i, j]);
                    }
                    
                    topPanel[j] = _panelBehaviours[current[j], j];
                    current[j]--;
                } 
            }
        }

        return isChanged ? sequence : null;
    }

    private void Reset() {
        for (int j = 0; j < _boardSize.y; j++) {
            for (int i = 0; i < _boardSize.x; i++) {
                if (_panelBehaviours[i, j] != null) _panelBehaviours[i, j].SetCanCombine(true);
            }
        }
    }

    public void MovePanels() {
        if (!_canInput) return;

        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            _moveTween = MoveDown()?.OnStart(() => {
                _canInput = false;
            })
            .OnComplete(() => {
                if (_generateNewPanel) GenerateNewPanel();
                Reset();
                _canInput = true;
            });
        }
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            _moveTween = MoveUp()?.OnStart(() => {
                _canInput = false;
            })
            .OnComplete(() => {
                if (_generateNewPanel) GenerateNewPanel();
                Reset();
                _canInput = true;
            });
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            _moveTween = MoveLeft()?.OnStart(() => {
                _canInput = false;
            })
            .OnComplete(() => {
                if (_generateNewPanel) GenerateNewPanel();
                Reset();
                _canInput = true;
            });
        }
        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            _moveTween = MoveRight()?.OnStart(() => {
                _canInput = false;
            })
            .OnComplete(() => {
                if (_generateNewPanel) GenerateNewPanel();
                Reset();
                _canInput = true;
            });
        }
    }

    public void KillPanelTween() => _moveTween?.Kill();
}
