using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;
using System.Text.RegularExpressions;

public class blindArrowsScript : MonoBehaviour 
{
	public KMAudio Audio;
	public KMBombInfo bomb;

	static int moduleIdCounter = 1;
	int moduleId;
	private bool moduleSolved;
	public KMSelectable[] matrix;

	public Material[] LEDOptions;
	public TextMesh[] Arrows;
	public Renderer LEDTop;
	public Renderer LEDRight;
    public Light[] lights;
	private int ButtonSolution = 0;
	private int LEDIndex1 = 0;
	private int LEDIndex2 = 0;
    private int ButtonIndex = 0;
	private int RotationStore = 0;
	private int Stage = 0;
    private bool lightsOn = false;

	void Awake()
	{
		moduleId = moduleIdCounter++;
        LEDTop.material = LEDOptions[0];
        LEDRight.material = LEDOptions[0];
        float scalar = transform.lossyScale.x;
        foreach (Light l in lights)
        {
            l.enabled = false;
            l.range *= scalar;
        }
        foreach (KMSelectable Arrow in matrix)
		{
			Arrow.OnInteract += delegate () { PressedArrow(Arrow); return false; };
		}
        GetComponent<KMBombModule>().OnActivate += OnActivate;
	}

	// Use this for initialization
	void Start () 
	{
		PickButtonCoordinate();
		RemoveArrows();
		PickLEDColor();	
		solving();
		Debug.LogFormat("[Blind Arrows #{0}] The correct button to press is {1}", moduleId, ButtonSolution + 1);
	}

    void OnActivate()
    {
        LEDTop.material = LEDOptions[LEDIndex1];
        LEDRight.material = LEDOptions[LEDIndex2];
        foreach (Light l in lights)
        {
            l.enabled = true;
        }
        lightsOn = true;
    }

	void PickLEDColor()
	{
		LEDIndex1 = UnityEngine.Random.Range(0,8);
        LEDIndex2 = UnityEngine.Random.Range(0,8);
        if (lightsOn)
        {
            LEDTop.material = LEDOptions[LEDIndex1];
            LEDRight.material = LEDOptions[LEDIndex2];
        }
        Debug.LogFormat("[Blind Arrows #{0}] The top LED is {1}, The right LED is {2}. ", moduleId, LEDOptions[LEDIndex1].name, LEDOptions[LEDIndex2].name);
	}

	void PickButtonCoordinate()
	{
        ButtonIndex = UnityEngine.Random.Range(0,25);
        Debug.LogFormat("[Blind Arrows #{0}] Initial button coordinates were {1}{2}.", moduleId, "ABCDE"[ButtonIndex % 5],"12345"[ButtonIndex / 5]);
	}

	void RemoveArrows()
	{
		for (int i = 0; i < Arrows.Length; i++)
		{
            Arrows[i].transform.localPosition = new Vector3(0f, 0.0123f, 0.0045f);
            Arrows[i].transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            if (i == ButtonIndex)
			{
				RotationStore = UnityEngine.Random.Range(0, 8);
                Arrows[i].transform.RotateAround(Arrows[i].transform.parent.position, new Vector3(0, -1, 0), 45f * RotationStore);
				continue;
			}
			Arrows[i].text = "";
		}
		Debug.LogFormat("[Blind Arrows #{0}] Initial rotation is {1}.", moduleId, "N NW W SW S SE E NE".Split(' ')[RotationStore]);
	}

	private int [][] NumTable =
	{
		new int []
		{
			33, 38, 61, 89, 62, 19, 02, 63 
		},
		new int []
		{
			81, 77, 53, 08, 45, 12, 05, 89 
		},
		new int []
		{
			54, 92, 73, 12, 28, 14, 65, 93 
		},
		new int []
		{
			58, 38, 41, 32, 16, 22, 13, 59 
		},
		new int []
		{
			48, 95, 83, 36, 22, 69, 98, 92 
		},
		new int []
		{
			72, 27, 16, 03, 35, 00, 71, 67  
		},
		new int []
		{
			97, 47, 40, 43, 66, 18, 80, 23 
		},
		new int []
		{
			49, 34, 84, 88, 98, 84, 06, 30 
		},
		new int []
		{
			65, 30, 25, 37, 71, 26, 76, 72 
		},
		new int []
		{
			46, 27, 76, 68, 11, 43, 75, 61 
		},
		new int []
		{
			78, 70, 34, 03, 51, 00, 13, 20  
		},
		new int []
		{
			09, 51, 39, 46, 29, 91, 56, 04 
		},
		new int []
		{
			19, 57, 04, 99, 42, 54, 58, 60 
		},
		new int []
		{
			68, 86, 82, 44, 40, 10, 57, 10 
		},
		new int []
		{
			33, 24, 86, 31, 74, 66, 01, 97 
		},
		new int []
		{
			88, 69, 44, 07, 62, 96, 94, 96 
		},
		new int []
		{
			49, 24, 28, 93, 99, 35, 17, 50 
		},
		new int []
		{
			36, 95, 08, 83, 14, 59, 55, 47 
		},
		new int []
		{
			79, 21, 80, 64, 02, 63, 94, 90 
		},
		new int []
		{
			77, 82, 87, 42, 05, 15, 15, 52 
		},
		new int []
		{
			56, 85, 41, 31, 25, 85, 74, 64 
		},
		new int []
		{
			45, 18, 06, 26, 07, 17, 52, 01 
		},
		new int []
		{
			39, 50, 55, 21, 11, 37, 29, 48 
		},
		new int []
		{
			91, 90, 60, 73, 78, 75, 70, 53 
		},
		new int []
		{
			32, 09, 20, 23, 81, 67, 87, 79
		}
	};
	private int [][][] RuleTable =
	{
		new int[][]
		{
			new int[]
			{
				00, 0
			},
			new int[]
			{
				05, 1
			},
			new int[]
			{
				10, 1
			},
			new int[]
			{
				15, 1
			},
			new int[]
			{
				20, 1
			},
			new int[]
			{
				25, 1
			},
			new int[]
			{
				30, 1
			},
			new int[]
			{
				35, 1
			}
		},
		new int[][]
		{
			new int[]
			{
				-05, 1
			},
			new int[]
			{
				00,1
			},
			new int[]
			{
				05, 2	
			},
			new int[]
			{
				00, 3
			},
			new int[]
			{
				25, 1
			},
			new int[]
			{
				00, 3
			},
			new int[]
			{
				-05, 2
			},
			new int[]
			{
				40, 1
			}
		},
		new int[][]
		{
			new int[]
			{
				-10, 1
			},
			new int[]
			{
				-05, 2
			},
			new int[]
			{
				00, 1
			},
			new int[]
			{
				05, 2
			},
			new int[]
			{
				30, 1
			},
			new int[]
			{
				35, 1
			},
			new int[]
			{
				40, 1
			},
			new int[]
			{
				45, 1
			}
		},
		new int[][]
		{
			new int[]
			{
				-15, 1
			},
			new int[]
			{
				00, 3
			},
			new int[]
			{
				-05, 2
			},
			new int[]
			{
				00, 1
			},
			new int[]
			{
				05, 2
			},
			new int[]
			{
				00, 3
			},
			new int[]
			{
				45, 1
			},
			new int[]
			{
				50, 1
			}
		},
		new int[][]
		{
			new int[]
			{
				-20, 1
			},
			new int[]
			{
				-25, 1
			},
			new int[]
			{
				-30, 1
			},
			new int[]
			{
				-05, 2
			},
			new int[]
			{
				00, 1
			},
			new int[]
			{
				05, 2
			},
			new int[]
			{
				50, 1
			},
			new int[]
			{
				55, 1
			}
		},
		new int[][]
		{
			new int[]
			{
				-25, 1
			},
			new int[]
			{
				00, 3
			},
			new int[]
			{
				-35, 1
			},
			new int[]
			{
				00, 3
			},
			new int[]
			{
				-05, 2
			},
			new int[]
			{
				00, 1
			},
			new int[]
			{
				05, 2
			},
			new int[]
			{
				60, 1
			}
		},
		new int[][]
		{
			new int[]
			{
				-30, 1
			},
			new int[]
			{
				05, 2
			},
			new int[]
			{
				-40, 1
			},
			new int[]
			{
				-45, 1
			},
			new int[]
			{
				-50, 1
			},
			new int[]
			{
				-05, 2
			},
			new int[]
			{
				00, 1
			},
			new int[]
			{
				65, 1
			}
		},
		new int[][]
		{
			new int[]
			{
				-35, 1
			},
			new int[]
			{
				-40, 1
			},
			new int[]
			{
				-45, 1
			},
			new int[]
			{
				-50, 1
			},
			new int[]
			{
				-55, 1
			},
			new int[]
			{
				-60, 1
			},
			new int[]
			{
				-65, 1
			},
			new int[]
			{
				99, 0
			}
		}
	};
	private int mod(int num, int m)
	{
		while (num < 0)
		{
			num = num + m;
		}
		return num % m;
	}
	void solving()
	{
		int number;
		switch(RuleTable[LEDIndex2][LEDIndex1][1])
		{
			case 0: number = RuleTable[LEDIndex2][LEDIndex1][0]; break;
			case 1: number = NumTable[ButtonIndex][RotationStore] + RuleTable[LEDIndex2][LEDIndex1][0]; break;
			case 2: number = NumTable[mod(ButtonIndex + RuleTable[LEDIndex2][LEDIndex1][0], 25)][RotationStore]; break;
			default: number = ((NumTable[ButtonIndex][RotationStore] % 10)*10) + (NumTable[ButtonIndex][RotationStore] / 10); break;
		}
		number = mod(number, 100);
		if (number == 0)
		{
			number = mod(bomb.GetSerialNumberNumbers().Sum(), 100);
		}
		Debug.LogFormat("[Blind Arrows #{0}] Requested number is {1}.", moduleId, number);
		if (bomb.GetIndicators().Any(x => x == "BOB"))
		{
			ButtonSolution = ButtonIndex;
			return;
		}
		if (number.InRange(1,20))
		{
			if (bomb.GetBatteryCount() > bomb.GetIndicators().Count())
			{
				ButtonSolution = GetNewButton(mod(RotationStore + 4, 8), 2);
				return;
			}
			ButtonSolution = GetNewButton(RotationStore, 2);
		}
		else if(number.InRange(21,40))
		{
			if (bomb.GetOnIndicators().Any(x => x.EqualsAny("IND", "FRQ", "NSA")))
			{
				ButtonSolution = GetNewButton(mod(RotationStore - 1, 8), 3);
				return;
			}
			ButtonSolution = GetNewButton(RotationStore, 3);
		}
		else if(number.InRange(41,60))
		{
			ButtonSolution = GetNewButton(mod(RotationStore + 4, 8), 1);
		}
		else if(number.InRange(61,80))
		{
			if(bomb.GetSolvedModuleNames().Count() > bomb.GetModuleNames().Count() / 2)
			{
				ButtonSolution = GetNewButton(mod(RotationStore + 4, 8), 5);
				return;
			}
			ButtonSolution = GetNewButton(RotationStore, 5);
		}
		else if(number.InRange(81,99))
		{
			ButtonSolution = GetNewButton(mod(RotationStore + 2, 8), 4);
		}
	}
	int GetNewButton(int Direction, int Amount)
	{
		int ModdedIndex = mod(ButtonIndex, 5);
		int[] BaseRow = new int[] { 0, 1, 2, 3, 4 };
		int[] BaseColumn = new int[] { 0, 5, 10, 15, 20 };
		int[] ColumnNums = new int[5];
		int[] RowNums = new int[5];
		int Column = mod(ButtonIndex, 5);
		int Row = ButtonIndex / 5;
		for (int i = 0; i <= 4; i++)
		{
			ColumnNums[i] = BaseColumn[i] + Column;
			RowNums[i] = BaseRow[i] + (Row * 5);
		}
		switch (Direction)
		{
			case 0:
				if (ButtonIndex.InRange(0, 4) && Amount == 1)
				{
					int[] dif = new int[] { 20, 21, 22, 23, 24 };
					return dif[Column];
				}
				return ColumnNums[mod(mod(Row, 5) - Amount, 5)];
			case 1:
				int n1 = ButtonIndex;
				for (int i = 0; i < Amount; i++)
				{
					if (n1 % 5 == 0)
					{
						n1--;
					}
					else
					{
						n1 -= 6;
					}
				}
				return mod(n1, 25);
			case 2:
				if (ButtonIndex.EqualsAny(0, 5, 10, 15, 20) && Amount == 1)
				{
					int[] dif = new int[] { 4, 9, 14, 19, 24 };
					return dif[Row];
				}
				return RowNums[mod(mod(Column, 5) - Amount, 5)];
			case 3:
				int n2 = ButtonIndex;
				for (int i = 0; i < Amount; i++)
				{
					if (n2 % 5 == 0)
					{
						n2 += 9;
					}
					else
					{
						n2 += 4;
					}
				}
				return mod(n2, 25);
			case 4:
				if (ButtonIndex.InRange(20, 24) && Amount == 1)
				{
					int[] dif = new int[] { 0, 1, 2, 3, 4 };
					return dif[Column];
				}
				return ColumnNums[mod(mod(Row, 5) + Amount, 5)];
			case 5:
				int n3 = ButtonIndex;
				for (int i = 0; i < Amount; i++)
				{
					if (n3 % 5 == 4)
					{
						n3++;
					}
					else
					{
						n3 += 6;
					}
				}
				return mod(n3, 25);
			case 6:
				if (ButtonIndex.EqualsAny(4, 9, 14, 19, 24) && Amount == 1)
				{
					int[] dif = new int[] { 0, 5, 10, 15, 20 };
					return dif[Row];
				}
				return RowNums[mod(mod(Column, 5) + Amount, 5)];
			default:
				int n4 = ButtonIndex;
				for (int i = 0; i < Amount; i++)
				{
					if (n4 % 5 == 4)
					{
						n4 -= 9;
					}
					else
					{
						n4 -= 4;
					}
				}
				return mod(n4, 25);
		}
	}
	void PressedArrow(KMSelectable Button)
	{
		Button.AddInteractionPunch();
		Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Button.transform);
		if(moduleSolved) return;
		int Index = Array.IndexOf(matrix, Button);
		if(Index != ButtonSolution)
		{
			GetComponent <KMBombModule>().HandleStrike();
			Debug.LogFormat("[Blind Arrows #{0}] Strike! Incorrect button press. Expected {1}. Got {2}.", moduleId, ButtonSolution + 1, Index + 1);
			return;
		}
		Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.CorrectChime, transform);
		if(Stage == 4)
		{
			GetComponent <KMBombModule>().HandlePass();
			moduleSolved = true;
			Debug.LogFormat("[Blind Arrows #{0}] Correct. The module is solved.", moduleId);
			return;
		}
		Debug.LogFormat("[Blind Arrows #{0}] Correct. Moving on to stage {1}.", moduleId, Stage + 2);
		GenerateStage(Index);
	}
	void GenerateStage(int NewIndex)
	{
		ButtonIndex = NewIndex;
		Arrows[ButtonIndex].text = "↑";
		PickLEDColor();
		RemoveArrows();
		solving();
		Debug.LogFormat("[Blind Arrows #{0}] The correct button to press is {1}", moduleId, ButtonSolution + 1);
		++Stage;
	}

    //twitch plays
    #pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} press <coord> [Presses the button at the specified coordinate] | Valid coordinates are A1-E5";
    #pragma warning restore 414
    IEnumerator ProcessTwitchCommand(string command)
    {
        string[] parameters = command.Split(' ');
        if (Regex.IsMatch(parameters[0], @"^\s*press\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            if (parameters.Length > 2)
            {
                yield return "sendtochaterror Too many parameters!";
            }
            else if (parameters.Length == 2)
            {
                string[] valids = new string[] { "A1", "B1", "C1", "D1", "E1", "A2", "B2", "C2", "D2", "E2", "A3", "B3", "C3", "D3", "E3", "A4", "B4", "C4", "D4", "E4", "A5", "B5", "C5", "D5", "E5" };
                if (valids.Contains(parameters[1].ToUpper()))
                {
                    matrix[Array.IndexOf(valids, parameters[1].ToUpper())].OnInteract();
                }
                else
                {
                    yield return "sendtochaterror The specified coordinate '" + parameters[1] + "' is invalid!";
                }
            }
            else if (parameters.Length == 1)
            {
                yield return "sendtochaterror Please specify a coordinate of a button to press!";
            }
            yield break;
        }
    }

    IEnumerator TwitchHandleForcedSolve()
    {
        int start = Stage;
        for (int i = start; i < 5; i++)
        {
            matrix[ButtonSolution].OnInteract();
            yield return new WaitForSeconds(0.1f);
        }
    }
}