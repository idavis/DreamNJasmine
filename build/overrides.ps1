﻿# 
# Copyright (c) 2011-2012, Toji Project Contributors
# 
# Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
# See the file LICENSE.txt for details.
# 

# This file is where you can override any settings specified in the other scripts.
# These settings can also be used to override conventions such as solution name and nuspec file.

properties {
  Write-Output "Loading override settings" 
  $msbuild.userparameters = @{BaseIntermediateOutputPath="$($base.dir)\obj.build\"
                              NUnitLibPath=$NUnitLibPath
							  NUnitFrameworkPath=$NUnitFrameworkPath
						}
  $nunit.runner = (join-path $NUnitBinPath nunit-console.exe)
  $nunit.gui = (join-path $build.dir "nunit\nunit.exe")
  $nunit.XmlTarget = (join-path $build.dir "UnitTests.xml")
}