return {
  ["context"] = {
    ["a"] = 1,
  },
  ["states"] = {
    ["Idle"] = {
      ["on"] = {
        ["move"] = {
          [1] = {
            ["target"] = "Move",
            ["actions"] = {
            },
            ["meta"] = {
            },
          },
        },
      },
      ["invoke"] = {
        ["src"] = "handleMove",
        ["input"] = {
        },
      },
      ["entry"] = {
        [1] = {
          ["type"] = "coreUpdateAnimator",
          ["params"] = {
            ["Idle"] = true,
          },
        },
        [2] = {
          ["type"] = "coreLogAction",
          ["params"] = {
            ["action"] = "Idle start",
          },
        },
      },
      ["exit"] = {
        ["type"] = "coreUpdateAnimator",
        ["params"] = {
          ["Idle"] = false,
        },
      },
    },
    ["Move"] = {
      ["invoke"] = {
        [1] = {
          ["src"] = "handleMove",
          ["input"] = {
          },
        },
        [2] = {
          ["src"] = "coreLogAction",
          ["input"] = {
            ["action"] = "handle move",
          },
        },
      },
      ["entry"] = {
        ["type"] = "coreUpdateAnimator",
        ["params"] = {
          ["Move"] = true,
        },
      },
      ["exit"] = {
        ["type"] = "coreUpdateAnimator",
        ["params"] = {
          ["Move"] = false,
        },
      },
    },
  },
  ["initial"] = "Idle",
  ["id"] = "WarriorPlayer",
}