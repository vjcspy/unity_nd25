package = "cli"
version = "dev-1"
source = {
   url = "git+ssh://git@github.com/vjcspy/unity_nd25.git"
}
description = {
   homepage = "*** please enter a project homepage ***",
   license = "*** please specify a license ***"
}
build = {
   type = "builtin",
   modules = {
      json_to_table = "json_to_table.lua"
   }
}
