#define ApplicationVersion GetFileVersion('..\installer\Logitech.exe')
#define ProductVersion GetStringFileInfo('..\installer\Logitech.exe', 'ProductVersion')
#define FindFolder(Path) \
    Local[0] = FindFirst(Path, faDirectory), \
    Local[0] ? AddBackslash(ExtractFileDir(Path)) + FindGetFileName(Local[0]) : Path
	
	
[Setup]
AppVerName=LogiLed
AppName=LogiLed (c) EvilSoft
VersionInfoVersion={#ApplicationVersion}
AppId=logiled
DefaultDirName={code:DefDirRoot}\LogiLed
Uninstallable=Yes
OutputDir=..\Installer
SetupIconFile=..\logitech\icon.ico


[Tasks]
Name: desktopicon; Description: "Create a &desktop icon"; GroupDescription: "Icons:"
Name: starticon; Description: "Create a &startmenu icon"; GroupDescription: "Icons:"


[Icons]
Name: "{commonprograms}\Logitech"; Filename: "{app}\\Logitech.exe"; Tasks: starticon
Name: "{commondesktop}\Logitech"; Filename: "{app}\\Logitech.exe"; Tasks: desktopicon


[Files]
Source: "{#FindFolder("..\Logitech\bin\Release\*")}\*.*"; Excludes: "*.pdb"; DestDir: "{app}"; Flags: overwritereadonly replacesameversion recursesubdirs createallsubdirs touch ignoreversion
Source: "readme.txt"; DestDir: "{app}";

[Run]
Filename: "{app}\Logitech.exe"; Description: "Launch Logitech"; Flags: postinstall nowait




[Setup]
UseSetupLdr=yes
DisableProgramGroupPage=yes
DiskSpanning=no
AppVersion={#ApplicationVersion}
VersionInfoProductTextVersion={#ApplicationVersion}
PrivilegesRequired=admin
DisableWelcomePage=Yes
ArchitecturesInstallIn64BitMode=x64
AlwaysShowDirOnReadyPage=Yes
DisableDirPage=No
OutputBaseFilename=LogiLedInstaller
InfoAfterFile=readme.txt


[UninstallDelete]
Type: filesandordirs; Name: {app}

[Languages]
Name: eng; MessagesFile: compiler:Default.isl

[Code]
function IsRegularUser(): Boolean;
begin
Result := not (IsAdminLoggedOn or IsPowerUserLoggedOn);
end;

function DefDirRoot(Param: String): String;
begin
if IsRegularUser then
Result := ExpandConstant('{localappdata}')
else
Result := ExpandConstant('{pf}')
end;

