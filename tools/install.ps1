param($installPath, $toolsPath, $package, $project)

$project.Object.References | Where-Object { $_.Name -eq "Interop.WIA" } |  ForEach-Object { $_.EmbedInteropTypes = $false }
$project.Object.References | Where-Object { $_.Name -eq "Interop.PortableDeviceApiLib" } |  ForEach-Object { $_.EmbedInteropTypes = $false }
$project.Object.References | Where-Object { $_.Name -eq "Interop.PortableDeviceTypesLib" } |  ForEach-Object { $_.EmbedInteropTypes = $false }

$configItem = $project.ProjectItems.Item("EDSDK.dll")

# set 'Copy To Output Directory' to 'Copy if newer'
$copyToOutput = $configItem.Properties.Item("CopyToOutputDirectory")

# Copy Always Always copyToOutput.Value = 1
# Copy if Newer copyToOutput.Value = 2  
$copyToOutput.Value = 2

# set 'Build Action' to 'Content'
$buildAction = $configItem.Properties.Item("BuildAction")
$buildAction.Value = 2

$configItem = $project.ProjectItems.Item("EdsImage.dll")

# set 'Copy To Output Directory' to 'Copy if newer'
$copyToOutput = $configItem.Properties.Item("CopyToOutputDirectory")

# Copy Always Always copyToOutput.Value = 1
# Copy if Newer copyToOutput.Value = 2  
$copyToOutput.Value = 2

# set 'Build Action' to 'Content'
$buildAction = $configItem.Properties.Item("BuildAction")
$buildAction.Value = 2