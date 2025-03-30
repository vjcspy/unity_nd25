return {
  ["states"] = {
    ["Idle"] = {
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
      ["on"] = {
        ["move"] = {
          [1] = {
            ["meta"] = {
            },
            ["target"] = "Move",
            ["actions"] = {
            },
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
      ["invoke"] = {
        ["input"] = {
        },
        ["src"] = "monoBehaviourCSharp:HandleUserInput",
      },
    },
    ["Move"] = {
      ["exit"] = {
        ["params"] = {
          ["move"] = false,
        },
        ["type"] = "monoBehaviourCSharp:UpdateAnimatorParams",
      },
      ["on"] = {
        ["idle"] = {
          [1] = {
            ["target"] = "Idle",
            ["actions"] = {
            },
          },
        },
      },
      ["entry"] = {
        [1] = {
          ["params"] = {
            ["yVelocity"] = 1.2,
            ["move"] = true,
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
      ["invoke"] = {
        ["input"] = {
        },
        ["src"] = "monoBehaviourCSharp:HandleUserInput",
      },
    },
  },
  ["id"] = "WarriorPlayer",
  ["context"] = {
    ["a"] = 1,
  },
  ["initial"] = "Idle",
}
