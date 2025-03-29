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
          ["type"] = "updateAnimator",
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
        ["type"] = "updateAnimator",
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
        ["type"] = "updateAnimator",
        ["params"] = {
          ["Move"] = true,
        },
      },
      ["exit"] = {
        ["type"] = "updateAnimator",
        ["params"] = {
          ["Move"] = false,
        },
      },
    },
  },
  ["initial"] = "Idle",
  ["id"] = "WarriorPlayer",
}