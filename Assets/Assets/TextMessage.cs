using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class TextMessage : MonoBehaviour
{
    [SerializeField]
    Text scenarioMessage;
    List<Scenario> scenarios = new List<Scenario>();

    [SerializeField]
    Transform buttonPanel;

    [SerializeField]
    Button optionButton;

    Scenario currentScenario;
    Scenario scenario01;
    Scenario scenario03;
    int index = 0;
    public static int BackToChoice = 0;
    HashSet<string> items = new HashSet<string>();

    class Scenario
    {
      public string ScenarioID;
      public List<string> Texts = new List<string>();
      public List<Option> Options = new List<Option>();
      public string NextScenarioID;
    }

    class Option
    {
      public string Text;
      public Action action;
      public Func<bool> IsFlagOK = () => {return true;};
    }

    Scenario scenario02;
    bool isCheckedKey = false;

    void Start()
    {
        var scenario01 = new Scenario()
        {
        	ScenarioID = "scenario01",
        	Texts = new List<string>()
        	{
        		"お、フリーWi-Fiが通ってるみたいだ。",
        		"データ通信の制限がかかりそうな手前だ、"
        	},
                NextScenarioID = "scenario02"
        	};

        var scenario02 = new Scenario()
        {
        	ScenarioID = "scenario02",
        	Texts = new List<string>()
        	{
        		"兎にも角にもWi-Fiに繋げた方がいいか？"
        	},
        	Options = new List<Option>
                {
                  new Option()
                  {
                    Text = "繋げる",
                    action = WifiConnect
                  },
                  new Option()
                  {
                    Text = "繋げない",
                    action = NotWifiConnect
                  }
                }
          };
          scenarios.Add(scenario02);

      var scenario03 = new Scenario()
      {
          ScenarioID = "scenario03",
          Texts = new List<string>()
          {
            "良し、取り敢えずWi-Fiを繋げたぞ。",
            "信頼できるかどうかは分からないが。"
          },
          NextScenarioID = "scenario04"
      };
      scenarios.Add(scenario03);


      var scenario04 = new Scenario()
      {
        ScenarioID = "scenario04",
        Texts = new List<string>()
        {
          "とにかくネットショッピングをしたいがどうしようか？",
        },
        Options = new List<Option>
        {
          new Option()
          {
            Text = "ログイン",
            action = RogIn
          },
          new Option()
          {
            Text = "ブラウジングだけ",
            action = Browsing
          },
          new Option()
          {
            Text = "繋げない",
            action = NotConnect
          }
        }
      };
      scenarios.Add(scenario04);

      SetScenario(scenario01);

    }

    public void WifiConnect()
    {
    	var scenario = new Scenario();
    	scenario.NextScenarioID = "scenario06";
      scenario.Texts.Add("まぁした方が良いよな。");
      var scenario06 = new Scenario()
      {
        ScenarioID = "scenario06",
        Texts = new List<string>()
        {
          "ちょうど店のSSIDが見えるな。これを参考に接続しようか。",
        },
        Options = new List<Option>
        {
          new Option()
          {
            Text = "Wi-Fiに接続する",
            action = SelectWiFi
          }
        }
      };
      scenarios.Add(scenario06);
      SetScenario(scenario);
    }

    public void SelectWiFi()
    {
      var scenario = new Scenario();
      scenario.NextScenarioID = "scenario03";
      scenario.Texts.Add("Wait...");
      BackToChoice++;
      SceneManager.LoadScene("SampleScene");
      SetScenario(scenario);
    }

    public void NotWifiConnect()
    {
    	var scenario = new Scenario();
      scenario.NextScenarioID = "scenario04";
    	scenario.Texts.Add("今は接続する必要が無いな…しかし何かあるかは分からない。");
    	scenario.Texts.Add("万が一の事を考えて、データ容量は節約しておきたい。");
    	SetScenario(scenario);
    }

    public void RogIn()
    {
      var scenario = new Scenario();
      scenario.NextScenarioID = "scenario05";
      scenario.Texts.Add("何の商品か確認取らないと変な物摑まされるかもしれないからな。");

      var scenario05 = new Scenario()
      {
        ScenarioID = "scenario05",
        Texts = new List<string>()
        {
          "いっその事、商品も買ってしまおうか？",
        },
        Options = new List<Option>
        {
          new Option()
          {
            Text = "カード番号の入力",
            action = CardNumber
          },
          new Option()
          {
            Text = "ブラウジング",
            action = Browsing
          }
        }
      };

      scenarios.Add(scenario05);
      SetScenario(scenario);
    }

    public void CardNumber()
    {
      var scenario = new Scenario();
      scenario.NextScenarioID = "scenario04";
      scenario.Texts.Add("善は急げだ！買ってしまおう。");
      SetScenario(scenario);
    }

    public void Browsing()
    {
      var scenario = new Scenario();
      scenario.NextScenarioID = "scenario04";
      scenario.Texts.Add("商品のスクショ撮って後で買えばいいか。");
      SetScenario(scenario);
    }

    public void NotConnect()
    {
      var scenario = new Scenario();
      scenario.NextScenarioID = "scenario04";
      scenario.Texts.Add("今はそういう時じゃないな。");
      SetScenario(scenario);
    }

    void Update()
    {
      if(currentScenario != null)
      {
        if(Input.GetMouseButtonDown(0) || Input.GetKeyDown("return"))
        {
          if(EventSystem.current.IsPointerOverGameObject())
          {
            return;
          }
          if(buttonPanel.GetComponentsInChildren<Button>().Length < 1)
          {
            SetNextMessage();
          }
        }
      }
    }

    void SetScenario(Scenario scenario)
    {
      currentScenario = scenario;
      scenarioMessage.text = currentScenario.Texts[0];
      if(currentScenario.Options.Count > 0)
      {
        SetNextMessage();
      }
    }

    void SetNextMessage()
    {
      if(currentScenario.Texts.Count > index + 1)
      {
        index++;
        scenarioMessage.text = currentScenario.Texts[index];
      }
      else
      {
        ExitScenario();
      }
    }

    void ExitScenario()
    {
      index = 0;

      if(currentScenario.Options.Count > 0)
      {
        SetOptions();
      }

      else
      {
        scenarioMessage.text = "";
        var nextScenario = scenarios.Find(s => s.ScenarioID == currentScenario.NextScenarioID);
        if(nextScenario != null)
        {
          SetScenario(nextScenario);
        }
        else
        {
          currentScenario = null;
        }
      }

    }

    void SetOptions()
    {
      foreach(Option o in currentScenario.Options)
      {
        if(o.IsFlagOK())
        {
          Button b = Instantiate(optionButton);
          Text text = b.GetComponentInChildren<Text>();
          text.text = o.Text;
          b.onClick.AddListener(() => o.action());
          b.onClick.AddListener(() => ClearButtons());
          b.transform.SetParent(buttonPanel, false);
        }
      }
    }

    void ClearButtons()
    {
      foreach(Transform t in buttonPanel)
      {
        Destroy(t.gameObject);
      }
    }

}
