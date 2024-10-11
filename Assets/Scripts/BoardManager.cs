using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [SerializeField] private PanelManager _panelManager;
    [SerializeField] private Vector2Int _boardSize;

    void Initialize() {
        _panelManager.KillPanelTween();
        _panelManager.Destroy();

        // 節ガジェット説明用
        // var _panelNumbers = new int[, ] {
        //     {2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1 },
        //     {1, 2, 1, 2, 1, 2, 1, 2, 1, 3, 4, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 3, 4, 2, 1, 2, 1, 2, 1, 2, 1, 2 },
        //     {2, 1, 2, 1, 2, 1, 2, 1, 2, 5, 6, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 5, 6, 1, 2, 1, 2, 1, 2, 1, 2, 1 },
        //     {1, 2, 1, 2, 1, 2, 1, 2, 3, 2, 1, 4, 1, 2, 1, 2, 1, 3, 5, 2, 1, 2, 1, 2, 3, 2, 1, 4, 1, 2, 1, 2, 1, 2, 1, 2 },
        //     {2, 1, 2, 1, 2, 1, 2, 1, 5, 1, 2, 6, 2, 1, 2, 1, 2, 4, 6, 1, 2, 1, 2, 1, 5, 1, 2, 6, 2, 1, 2, 1, 2, 1, 2, 1 },
        //     {1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2 },
        //     {2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1 },
        //     {1, 2, 1, 2, 1, 2, 1, 2, 1, 3, 5, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 3, 5, 2, 1, 2, 1, 2, 1, 2, 1, 2 },
        //     {2, 3, 3, 4, 4, 1, 2, 3, 5, 1, 2, 3, 5, 1, 2, 1, 2, 3, 5, 1, 2, 1, 2, 3, 5, 1, 2, 3, 5, 1, 2, 3, 3, 4, 4, 1 },
        //     {1, 5, 5, 6, 6, 2, 1, 4, 6, 2, 1, 4, 6, 2, 1, 2, 1, 4, 6, 2, 1, 2, 1, 4, 6, 2, 1, 4, 6, 2, 1, 5, 5, 6, 6, 2 },
        //     {2, 1, 2, 1, 2, 1, 2, 1, 2, 4, 6, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 4, 6, 1, 2, 1, 2, 1, 2, 1, 2, 1 },
        //     {1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2 },
        // };

        // 充足判定ガジェット説明用
        // var _panelNumbers = new int[, ] {
        //     {1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2},
        //     {2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1},
        //     {1, 2, 6, 2, 1, 4, 6, 2, 1, 4, 6, 2, 1, 4, 1, 2},
        //     {2, 1, 3, 1, 2, 5, 3, 1, 2, 5, 3, 1, 2, 5, 2, 1},
        //     {1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2},
        //     {2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1},
        //     {1, 2, 1, 6, 4, 2, 1, 6, 4, 2, 1, 6, 4, 2, 1, 2},
        //     {2, 1, 2, 3, 5, 1, 2, 3, 5, 1, 2, 3, 5, 1, 2, 1},
        //     {1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2},
        //     {2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1},
        //     {1, 3, 5, 2, 1, 3, 5, 2, 1, 3, 5, 2, 1, 3, 5, 2},
        //     {2, 3, 5, 1, 2, 3, 5, 1, 2, 3, 5, 1, 2, 3, 5, 1},
        //     {1, 4, 6, 2, 1, 4, 6, 2, 1, 4, 6, 2, 1, 4, 6, 2},
        //     {2, 4, 6, 1, 2, 4, 6, 1, 2, 4, 6, 1, 2, 4, 6, 1},
        //     {1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2},
        //     {2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1},
        //     {1, 3, 5, 2, 1, 3, 5, 2, 1, 3, 5, 2, 1, 3, 5, 2},
        //     {2, 3, 5, 1, 2, 3, 5, 1, 2, 3, 5, 1, 2, 3, 5, 1},
        //     {1, 4, 6, 2, 1, 4, 6, 2, 1, 4, 6, 2, 1, 4, 6, 2},
        //     {2, 4, 6, 1, 2, 4, 6, 1, 2, 4, 6, 1, 2, 4, 6, 1},
        //     {1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2},
        // };

        // _boardSize = new Vector2Int(_panelNumbers.GetLength(1), _panelNumbers.GetLength(0));

        _panelManager.Initialize(_boardSize/*, _panelNumbers*/);

        transform.localScale = new Vector3(_boardSize.x, _boardSize.y);
        Camera.main.orthographicSize = Mathf.Max(_boardSize.x * 0.3f, _boardSize.y * 0.5f);
    }

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        _panelManager.MovePanels();

        if (Input.GetKeyDown(KeyCode.A)) {
            _boardSize.x--;
            if (_boardSize.x <= 0) _boardSize.x = 1;
            Initialize();
        }
        if (Input.GetKeyDown(KeyCode.D)) {
            _boardSize.x++;
            Initialize();
        }
        if (Input.GetKeyDown(KeyCode.S)) {
            _boardSize.y--;
            if (_boardSize.y <= 0) _boardSize.y = 1;
            Initialize();
        }
        if (Input.GetKeyDown(KeyCode.W)) {
            _boardSize.y++;
            Initialize();
        }
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Initialize();
        }
    }
}
