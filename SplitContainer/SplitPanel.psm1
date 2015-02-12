Import-Module $psscriptroot\SplitPanel.dll
$psISE.CurrentPowerShellTab.VerticalAddOnTools.Add(‘SplitPanel’, [SplitPanel.AddinContainer], $true)