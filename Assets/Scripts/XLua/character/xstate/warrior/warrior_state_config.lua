return {
  ["states"] = {
    ["Move"] = {
      ["entry"] = {
        [1] = {
          ["params"] = {
            ["Move"] = true,
          },
          ["type"] = "coreUpdateAnimator",
        },
        [2] = {
          ["params"] = {
            ["currentState"] = "Move",
            ["action"] = "Move State Start",
          },
          ["type"] = "coreLogAction",
        },
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
      ["exit"] = {
        ["params"] = {
          ["Move"] = false,
        },
        ["type"] = "coreUpdateAnimator",
      },
      ["invoke"] = {
        ["src"] = "coreHandleMove",
        ["input"] = {
        },
      },
    },
    ["Idle"] = {
      ["entry"] = {
        [1] = {
          ["params"] = {
            ["Idle"] = true,
          },
          ["type"] = "coreUpdateAnimator",
        },
        [2] = {
          ["params"] = {
            ["currentState"] = "Idle",
            ["action"] = "Idle State Start",
          },
          ["type"] = "coreLogAction",
        },
      },
      ["on"] = {
        ["move"] = {
          [1] = {
            ["target"] = "Move",
            ["meta"] = {
            },
            ["actions"] = {
            },
          },
        },
      },
      ["exit"] = {
        [1] = {
          ["params"] = {
            ["Idle"] = false,
          },
          ["type"] = "coreUpdateAnimator",
        },
        [2] = {
          ["params"] = {
            ["action"] = "Exit Idle State",
          },
          ["type"] = "coreLogAction",
        },
      },
      ["invoke"] = {
        ["src"] = "coreHandleMove",
        ["input"] = {
        },
      },
    },
  },
  ["context"] = {
    ["a"] = 1,
  },
  ["id"] = "WarriorPlayer",
  ["initial"] = "Idle",
}