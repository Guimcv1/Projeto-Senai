[Setup]
; Nome do aplicativo que aparecerá no Painel de Controle e Menu Iniciar
AppName=SCA
AppVersion=1.0.0
AppPublisher=SENAI
; Pasta padrão de instalação (Arquivos de Programas)
DefaultDirName={autopf}\SCA
DefaultGroupName=SCA
; Ícone para desinstalação
UninstallDisplayIcon={app}\SCA.exe
Compression=lzma2
SolidCompression=yes
; Local onde o instalador final 
OutputDir=C:\Users\nunes\Desktop\Nova pasta
OutputBaseFilename=Instalador_SCA
; Define compatibilidade mínima (6.1 é Windows 7)
MinVersion=6.1 

[Files]
; O executável principal gerado pelo seu Build do C#
Source: "C:\Users\nunes\Desktop\gereciador_chaves_senai\SCA\bin\BildInstaler\SCA.exe"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
; Atalhos do sistema
Name: "{group}\SCA"; Filename: "{app}\SCA.exe"
Name: "{autodesktop}\SCA"; Filename: "{app}\SCA.exe"; Tasks: desktopicon

[Tasks]
Name: "desktopicon"; Description: "Criar um ícone na Área de Trabalho"; GroupDescription: "Ícones Adicionais:"

[Code]
var
  DBPage: TInputQueryWizardPage;
  AdminPage: TInputQueryWizardPage;

//Inicialização das Telas Personalizadas 
procedure InitializeWizard;
begin
  // Cria a tela de Configuração do Banco de Dados
  DBPage := CreateInputQueryPage(wpSelectDir,
    'Configuração do Banco de Dados', 'Configure as variáveis do sistema (.env)',
    'Por favor, insira os dados de conexão do banco PostgreSQL:');
    
  DBPage.Add('DB_HOST:', False);     
  DBPage.Add('DB_PORT:', False);    
  DBPage.Add('DB_USER:', False);    
  DBPage.Add('DB_SENHA:', True);    
  DBPage.Add('DB_NAME:', False);     

  // Valores padrão para :
  DBPage.Values[0] := 'localhost'; // DB_HOST
  DBPage.Values[1] := '5432'; // DB_PORT

  // Cria a tela de Configuração do Administrador
  AdminPage := CreateInputQueryPage(DBPage.ID,
    'Configuração do Usuário Administrador', 'Configure as credenciais padrão',
    'Insira o login e senha do administrador do sistema:');
    
  AdminPage.Add('USER_ADMIN_LOGIN:', False); 
  AdminPage.Add('USER_ADMIN_SENHA:', True);  
end;

// Validação: Impede avançar se houver campos vazios 
function NextButtonClick(CurPageID: Integer): Boolean;
begin
  Result := True; 

  // Validação da página do Banco
  if CurPageID = DBPage.ID then
  begin
    if (Trim(DBPage.Values[0]) = '') or (Trim(DBPage.Values[2]) = '') or 
       (Trim(DBPage.Values[3]) = '') or (Trim(DBPage.Values[4]) = '') then
    begin
      MsgBox('Erro: Por favor, preencha todos os campos do banco de dados.', mbError, MB_OK);
      Result := False;
    end;
  end;

  // Validação da página do Admin
  if CurPageID = AdminPage.ID then
  begin
    if (Trim(AdminPage.Values[0]) = '') or (Trim(AdminPage.Values[1]) = '') then
    begin
      MsgBox('Erro: O login e senha do administrador são obrigatórios.', mbError, MB_OK);
      Result := False;
    end;
  end;
end;

// Escrita do Arquivo .env após a instalação dos arquivos 
procedure CurStepChanged(CurStep: TSetupStep);
var
  EnvPath: string;
  EnvLines: TArrayOfString;
begin
  // Roda após os arquivos serem copiados para a pasta {app}
  if CurStep = ssPostInstall then
  begin
    EnvPath := ExpandConstant('{app}\.env');
    
    // Define o tamanho do array
    SetArrayLength(EnvLines, 8);

    // Mapeia os valores das telas para as linhas do arquivo
    EnvLines[0] := 'DB_HOST=' + Trim(DBPage.Values[0]);
    EnvLines[1] := 'DB_PORT=' + Trim(DBPage.Values[1]);
    EnvLines[2] := 'DB_USER=' + Trim(DBPage.Values[2]);
    EnvLines[3] := 'DB_SENHA=' + Trim(DBPage.Values[3]);
    EnvLines[4] := 'DB_NAME=' + Trim(DBPage.Values[4]);
    EnvLines[5] := ''; 
    EnvLines[6] := 'USER_ADMIN_LOGIN=' + Trim(AdminPage.Values[0]);
    EnvLines[7] := 'USER_ADMIN_SENHA=' + Trim(AdminPage.Values[1]);

    // Salva o arquivo fisicamente na pasta de instalação
    SaveStringsToFile(EnvPath, EnvLines, False);
  end;
end;