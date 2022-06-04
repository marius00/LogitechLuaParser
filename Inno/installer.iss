#define ApplicationVersion GetFileVersion('..\installer\KST.exe')
#define ProductVersion GetStringFileInfo('..\installer\KST.exe', 'ProductVersion')
#define FindFolder(Path) \
    Local[0] = FindFirst(Path, faDirectory), \
    Local[0] ? AddBackslash(ExtractFileDir(Path)) + FindGetFileName(Local[0]) : Path
	
	
[Setup]
AppVerName=KST
AppName=KST (c) EvilSoft
VersionInfoVersion={#ApplicationVersion}
AppId=kst
DefaultDirName={code:DefDirRoot}\KST
Uninstallable=Yes
OutputDir=..\Installer
SetupIconFile=..\kst\icon.ico


[Tasks]
Name: desktopicon; Description: "Create a &desktop icon"; GroupDescription: "Icons:"
Name: starticon; Description: "Create a &startmenu icon"; GroupDescription: "Icons:"


[Icons]
Name: "{commonprograms}\KST"; Filename: "{app}\\KST.exe"; Tasks: starticon
Name: "{commondesktop}\KST"; Filename: "{app}\\KST.exe"; Tasks: desktopicon


[Files]
Source: "{#FindFolder("..\KST\bin\Release\*")}\*.*"; Excludes: "*.pdb"; DestDir: "{app}"; Flags: overwritereadonly replacesameversion recursesubdirs createallsubdirs touch ignoreversion
Source: "readme.txt"; DestDir: "{app}";

[Run]
Filename: "{app}\KST.exe"; Description: "Launch KST"; Flags: postinstall nowait




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
OutputBaseFilename=KSTInstaller
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

