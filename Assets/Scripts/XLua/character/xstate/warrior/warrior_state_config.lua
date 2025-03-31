return {
  ["states"] = {
    ["Idle"] = {
      ["invoke"] = {
        [1] = {
          ["input"] = {
          },
          ["src"] = "monoBehaviourCSharp:HandleActionMove",
        },
        [2] = {
          ["input"] = {
          },
          ["src"] = "monoBehaviourCSharp:HandleActionJump",
        },
      },
      ["on"] = {
        ["move"] = {
          [1] = {
            ["actions"] = {
            },
            ["target"] = "Move",
          },
        },
        ["jump"] = {
          [1] = {
            ["actions"] = {
            },
            ["target"] = "Jump",
          },
        },
      },
      ["entry"] = {
        [1] = {
          ["params"] = {
            ["idle"] = true,
          },
          ["type"] = "monoBehaviourCSharp:UpdateAnimatorParams",
        },
        [2] = {
          ["params"] = {
            ["action"] = "Idle State Start",
            ["currentState"] = "Idle",
          },
          ["type"] = "coreLogAction",
        },
      },
      ["exit"] = {
        [1] = {
          ["params"] = {
            ["idle"] = false,
          },
          ["type"] = "monoBehaviourCSharp:UpdateAnimatorParams",
        },
        [2] = {
          ["params"] = {
            ["action"] = "Exit Idle State",
          },
          ["type"] = "coreLogAction",
        },
      },
    },
    ["Jump"] = {
      ["invoke"] = {
        [1] = {
          ["input"] = {
          },
          ["src"] = "monoBehaviourCSharp:HandleActionMove",
        },
        [2] = {
          ["input"] = {
          },
          ["src"] = "monoBehaviourCSharp:HandleFall",
        },
        [3] = {
          ["input"] = {
          },
          ["src"] = "monoBehaviourCSharp:CheckAndUpdateGroundedInfo",
        },
      },
      ["on"] = {
        ["grounded"] = {
          [1] = {
            ["actions"] = {
            },
            ["target"] = "Idle",
          },
        },
      },
      ["entry"] = {
        [1] = {
          ["params"] = {
            ["jump"] = true,
          },
          ["type"] = "monoBehaviourCSharp:UpdateAnimatorParams",
        },
        [2] = {
          ["type"] = "monoBehaviourCSharp:ForceJump",
        },
      },
      ["exit"] = {
        ["params"] = {
          ["jump"] = false,
        },
        ["type"] = "monoBehaviourCSharp:UpdateAnimatorParams",
      },
    },
    ["Move"] = {
      ["invoke"] = {
        [1] = {
          ["input"] = {
          },
          ["src"] = "monoBehaviourCSharp:HandleActionMove",
        },
        [2] = {
          ["input"] = {
          },
          ["src"] = "monoBehaviourCSharp:HandleActionJump",
        },
      },
      ["on"] = {
        ["idle"] = {
          [1] = {
            ["actions"] = {
            },
            ["target"] = "Idle",
          },
        },
        ["jump"] = {
          [1] = {
            ["actions"] = {
            },
            ["target"] = "Jump",
          },
        },
      },
      ["entry"] = {
        [1] = {
          ["params"] = {
            ["move"] = true,
            ["yVelocity"] = 1.2,
          },
          ["type"] = "monoBehaviourCSharp:UpdateAnimatorParams",
        },
        [2] = {
          ["params"] = {
            ["action"] = "Move State Start",
            ["currentState"] = "Move",
          },
          ["type"] = "coreLogAction",
        },
      },
      ["exit"] = {
        ["params"] = {
          ["move"] = false,
        },
        ["type"] = "monoBehaviourCSharp:UpdateAnimatorParams",
      },
    },
  },
  ["context"] = {
    ["a"] = 1,
  },
  ["id"] = "WarriorPlayer",
  ["initial"] = "Idle",
}
