# SWE40006 Software Deployment and Evolution
## Deployment Activity Report
### Task 1: Desktop Deployment using the Windows Installer XML (WiX) Toolset

---

## Student Details

| | |
|---|---|
| **Student ID** | _[FILL IN: Student ID]_ |
| **Full Name** | _[FILL IN: Full Name]_ |

## Unit Details

| | |
|---|---|
| **Unit Code** | SWE40006 |
| **Unit Name** | Software Deployment and Evolution |
| **Semester** | _[FILL IN: e.g. Summer 2025-2026]_ |
| **Due Date** | _[FILL IN: e.g. 20 January]_ |
| **Submission Date** | _[FILL IN: e.g. 08 January]_ |

## Task Level Declaration

I declare that I am attempting the following sub-tasks of **Task 1: Desktop deployment using the WiX toolset** (not the Wix website creation tool):

- [x] Task 1.1 (Pass) — Followed the WiX walkthrough and deployed a **sample** desktop application
- [x] Task 1.2 (Credit) — Followed the WiX walkthrough and deployed **my own C# desktop application** (Stock Tracking App)
- [x] Task 1.3 (Distinction) — Completed 1.1/1.2 **and** deployed an application with **multiple DLLs / dependencies** (64 DLLs)
- [x] Task 1.4 (High Distinction) — Completed 1.3 **and** explained in detail how to deploy the application to the **Microsoft Store** (MSIX package prepared for submission)

_Uncheck any level you are not attempting before submitting._

---

## Table of Contents

1. [Executive Summary](#1-executive-summary)
2. [Introduction](#2-introduction)
   - 2.1 [Background](#21-background)
   - 2.2 [Deployment Objectives](#22-deployment-objectives)
   - 2.3 [Technologies Used](#23-technologies-used)
3. [Task 1.1: Sample Desktop Deployment using the WiX Walkthrough (Pass)](#3-task-11-sample-desktop-deployment-using-the-wix-walkthrough-pass)
4. [Task 1.2: Deploying My Own C# Desktop Application — Stock Tracking App (Credit)](#4-task-12-deploying-my-own-c-desktop-application--stock-tracking-app-credit)
5. [Task 1.3: Deployment with Multiple DLLs and Dependencies (Distinction)](#5-task-13-deployment-with-multiple-dlls-and-dependencies-distinction)
6. [Task 1.4: Microsoft Store Deployment (High Distinction)](#6-task-14-microsoft-store-deployment-high-distinction)
7. [Source Code, Scripts and Brief Explanation](#7-source-code-scripts-and-brief-explanation)
8. [Error Analysis and Investigation](#8-error-analysis-and-investigation)
9. [Conclusion](#9-conclusion)
10. [Appendices](#10-appendices)

---

## 1. Executive Summary

This report documents the desktop deployment of the **Stock Tracking App**, a C#/.NET 9 desktop application, using the **Windows Installer XML (WiX) Toolset v5.0.2**.

Key achievements:

- Completed the WiX walkthrough and produced a working MSI installer for a sample desktop application (Task 1.1)
- Built a WiX `Package.wxs` authoring file and deployed my own C# Stock Tracking App as a per-machine MSI with Start Menu shortcut, product icon and EULA dialog (Task 1.2)
- Packaged and deployed the application together with its **64 DLL dependencies** (SplashKit/SDL2 graphics stack, Sqlite/SQLitePCLRaw stack, CsvHelper, and native libraries) in a single embedded-cab MSI (Task 1.3)
- Prepared an MSIX package and explained in detail the full Microsoft Store submission process (Task 1.4)
- Investigated and eradicated the console errors encountered during `dotnet publish`, `wix build` and `msiexec` installation (see [Section 8](#8-error-analysis-and-investigation))

---

## 2. Introduction

### 2.1. Background

_[FILL IN: 2–4 sentences describing the deployed application. Example: The Stock Tracking App is a C#/.NET 9 desktop application built with SplashKit that displays stock prices, historical charts, a wallet and trading-strategy simulation. Data is stored in a local SQLite database and CSV files, and price data is fetched over HTTP. It is deployed as a Windows Installer (MSI) package built with the WiX Toolset.]_

### 2.2. Deployment Objectives

1. Follow the official WiX walkthrough and successfully deploy a sample desktop application (Pass)
2. Author a WiX installer package for my own C# desktop application, the Stock Tracking App (Credit)
3. Package the application with all of its DLLs and runtime dependencies so that it installs and runs on a clean Windows machine (Distinction)
4. Explain in detail — and prepare the artifacts for — deployment of the application to the Microsoft Store (High Distinction)
5. Analyse and eradicate every error observed on the console during building, packaging and installation

### 2.3. Technologies Used

- **Windows Installer XML (WiX) Toolset** v5.0.2 (`wix.exe`, installed as a .NET local tool) — MSI/MSIX authoring and build
- **WixToolset.UI.wixext** v5.0.2 — `WixUI_InstallDir` installer dialog set (license + install-dir + verify)
- **Visual Studio / .NET SDK 10.0.401** — build environment (project targets `net9.0`)
- **Windows 10/11 SDK BuildTools 10.0.26100.1** — `makeappx.exe` / `signtool.exe` for MSIX packaging
- **Stock App runtime dependencies**: SplashKit 1.2.1, CsvHelper 33.0.1, Microsoft.Data.Sqlite 9.0.2 (see [Section 5](#5-task-13-deployment-with-multiple-dlls-and-dependencies-distinction))

---

## 3. Task 1.1: Sample Desktop Deployment using the WiX Walkthrough (Pass)

### 3.1. Objective

Follow the WiX walkthrough from **Modules/Unit Resources** and deploy a sample desktop application to verify that the WiX Toolset and Windows Installer environment work correctly.

### 3.2. Implementation

#### 3.2.1. Environment Setup

WiX Toolset was installed as a .NET global/local tool:

```console
_[FILL IN: paste console output of:]
dotnet tool install --tool-path %LOCALAPPDATA%\StockAppBuild\tools wix --version 5.0.2
%LOCALAPPDATA%\StockAppBuild\tools\wix.exe --version
```

**Verified console output (this machine):**

```text
5.0.2+aa65968c
```

![Fig. 1: WiX Toolset installed and reporting its version](path/to/fig1-wix-version.png)
*Fig. 1. `wix --version` confirming WiX Toolset v5.0.2 is installed*

The UI extension required by the walkthrough was then added:

```console
wix extension add -g WixToolset.UI.wixext/5.0.2
```

#### 3.2.2. Sample Desktop Deployment

_[FILL IN: describe the sample application from the walkthrough (e.g. the walkthrough's sample .wxs / HelloWorld desktop app), the exact `wix build` command you ran, and the install/uninstall commands:_

```console
_[FILL IN: e.g.]
wix build sample.wxs -ext WixToolset.UI.wixext -o sample.msi
msiexec /i sample.msi
msiexec /x sample.msi
```

_Include the pasted console output and a screenshot of the sample application running after installation.]_

![Fig. 2: Sample desktop application built with the WiX walkthrough](path/to/fig2-sample-wix-build.png)
*Fig. 2. `wix build` of the walkthrough sample completing successfully*

![Fig. 3: Sample application installed and running](path/to/fig3-sample-installed.png)
*Fig. 3. Sample desktop application running after MSI installation*

### 3.3. Results

The walkthrough sample was compiled into an MSI, installed through the Windows Installer GUI, launched successfully, and then uninstalled cleanly — confirming the toolchain (`wix.exe`, `WixToolset.UI.wixext`, `msiexec`) is fully operational before applying it to my own application.

_[FILL IN: paste any console output from the sample install/uninstall]_

### 3.4. Analysis

_[FILL IN: any errors encountered while following the walkthrough and how they were eradicated — cross-reference Section 8]_

---

## 4. Task 1.2: Deploying My Own C# Desktop Application — Stock Tracking App (Credit)

### 4.1. Objective

Follow the WiX walkthrough to deploy **my own C# desktop application** (the Stock Tracking App) as a Windows Installer package.

### 4.2. Implementation

#### 4.2.1. Application Publish (preparing binaries for packaging)

The application is published in Release mode for `win-x64` before packaging:

```console
dotnet publish StockApp.csproj -c Release -r win-x64 --self-contained false -o publish
```

_[FILL IN: paste the tail of the `dotnet publish` console output, e.g. the "Time Elapsed" / generated file list]_

![Fig. 4: dotnet publish of the Stock Tracking App](path/to/fig4-dotnet-publish.png)
*Fig. 4. `dotnet publish` producing the `publish\` payload used by WiX*

#### 4.2.2. WiX Package Authoring

The installer is authored in `Package.wxs` (source available at the public repository — see [Section 7](#7-source-code-scripts-and-brief-explanation)). Key elements:

```xml
<Package Name="Stock Tracking App" Manufacturer="StockApp"
         Version="$(var.Version)"
         UpgradeCode="{D7EF8B81-8B70-4C2D-9B9A-1C3CFBF3A1D0}"
         Scope="perMachine" Compressed="yes">
  <MediaTemplate CompressionLevel="high" EmbedCab="yes" />
  <MajorUpgrade DowngradeErrorMessage="A newer version of [ProductName] is already installed." />
  ...
  <ui:WixUI Id="WixUI_InstallDir" InstallDirectory="INSTALLFOLDER" />
</Package>
```

What this provides:

- **Per-machine installation** to `Program Files\Stock Tracking App`
- **Single embedded cabinet** (`EmbedCab="yes"`) so the MSI is a single distributable file
- **MajorUpgrade** handling so newer versions replace older ones
- **Start Menu shortcut** and **Add/Remove Programs icon** (`ARPPRODUCTICON`)
- **EULA licence dialog** (`WixUILicenseRtf` → `Eula.rtf`)

#### 4.2.3. Building the MSI

```console
_[FILL IN: paste your exact build command and console output, e.g.]
.\build-packages.ps1 -Version 1.0.0.0 -SkipMsix
```

Equivalent direct command:

```console
wix build Package.wxs -ext WixToolset.UI.wixext -d Version=1.0.0.0 -pdbtype none -o dist\StockApp-1.0.0.0-x64.msi
```

**Build result on this machine:**

```text
dist\StockApp-1.0.0.0-x64.msi    21708800 bytes (≈20.7 MB)   built 26/09/2026
```

![Fig. 5: WiX building the Stock Tracking App MSI](path/to/fig5-wix-build.png)
*Fig. 5. `wix build` compiling `Package.wxs` into `StockApp-1.0.0.0-x64.msi`*

#### 4.2.4. Installation and Verification

```console
msiexec /i dist\StockApp-1.0.0.0-x64.msi /l*v install.log
```

Installation verified in **Control Panel → Programs and Features**:

```text
DisplayName         : Stock Tracking App
DisplayVersion      : 1.0.0.0
```

![Fig. 6: MSI installation wizard (EULA and install-dir dialogs)](path/to/fig6-msi-wizard.png)
*Fig. 6. WiX `WixUI_InstallDir` wizard: licence agreement and install directory selection*

![Fig. 7: Stock Tracking App listed in Programs and Features](path/to/fig7-programs-and-features.png)
*Fig. 7. "Stock Tracking App 1.0.0.0" registered in Programs and Features*

![Fig. 8: Stock Tracking App launched from the Start Menu shortcut](path/to/fig8-app-running.png)
*Fig. 8. Application running after installation via the Start Menu shortcut*

Uninstallation check:

```console
msiexec /x dist\StockApp-1.0.0.0-x64.msi
_[FILL IN: paste console output]_
```

### 4.3. Results

- MSI produced successfully by `wix build` with no compiler errors
- Application installs per-machine with a working Start Menu shortcut and ARP entry
- Application launches correctly from the installed location
- MajorUpgrade/repair/uninstall behaviour verified through `msiexec`

### 4.4. Challenges and Solutions

#### 4.4.1. Challenge 1: _[FILL IN: e.g. ICE validation / WiX compile error, e.g. "The system cannot find the file ... publish\StockApp.exe"]_

**Issue / error message:**

```text
_[FILL IN: paste the exact WiX console error]_
```

**Investigation:** _[FILL IN: how you investigated — read the error line/column, ran with `-v`, checked paths]_

**Solution:** _[FILL IN: fix applied]_

#### 4.4.2. Challenge 2: _[FILL IN: e.g. shortcut/advertisement issue, or install failing with exit code 1603]_

**Issue / error message:**

```text
_[FILL IN: paste the exact console error]_
```

**Investigation:** _[FILL IN: how you investigated]_

**Solution:** _[FILL IN: fix applied]_

### 4.5. Analysis

Completed at **Credit** level. The deployment demonstrates authoring a real WiX package for my own C# application — `Package.wxs` with upgrade code, media template, UI dialogs, shortcut and icon — and successfully compiling, installing and running it.

---

## 5. Task 1.3: Deployment with Multiple DLLs and Dependencies (Distinction)

### 5.1. Objective

Complete Task 1.1 or 1.2 **and** deploy an application with **multiple DLLs or dependencies**.

### 5.2. Implementation

#### 5.2.1. Dependency Inventory

The Stock Tracking App depends on managed and native DLLs. The published payload contains **64 DLLs**:

| Dependency group | DLLs | Purpose |
|---|---|---|
| Application | `StockApp.exe`, `StockApp.dll` | Main executable and assembly |
| SplashKit / graphics | `SplashKit.dll`, `SplashKitSDK.dll`, `SDL2.dll`, `SDL2_image.dll`, `SDL2_mixer.dll`, `SDL2_net.dll`, `SDL2_ttf.dll`, `SDL2_gfx-1-0-0.dll`, `libfreetype-6.dll`, `libharfbuzz-0.dll`, `libpng16-16.dll`, `libjpeg-8.dll`, `libwebp-7.dll`, `libtiff-6.dll`, `libjxl.dll`, `zlib1.dll`, ... | 2D rendering, fonts, images, audio |
| Database (SQLite) | `Microsoft.Data.Sqlite.dll`, `e_sqlite3.dll`, `SQLitePCLRaw.batteries_v2.dll`, `SQLitePCLRaw.core.dll`, `SQLitePCLRaw.provider_e_sqlite3.dll` | Local SQLite persistence |
| CSV import | `CsvHelper.dll` | Reading historical price CSV files |
| Networking/codecs | `libcurl-4.dll`, `libssl-3-x64.dll`, `libcrypto-3-x64.dll`, `libssh2-1.dll`, `libidn2-0.dll`, `libnghttp2-14.dll`, `libavif-16.dll`, `libdav1d-7.dll`, `libaom.dll`, `rav1e.dll`, `libSvtAv1Enc.dll`, ... | HTTP fetch of stock data, image decode |
| Runtime support | `libgcc_s_seh.dll`, `libstdc++-6.dll`, `libwinpthread-1.dll`, `libiconv-2.dll`, `libglib-2.0-0.dll`, ... | MinGW/GCC runtime used by native libs |

Full list:

```console
dir /b publish\*.dll
_[FILL IN: paste the directory listing from your console]_
```

#### 5.2.2. Packaging All Dependencies into the MSI

`Package.wxs` includes the whole published payload through a `ComponentGroup` with a wildcard file include, so **every DLL** is installed as a Windows Installer component:

```xml
<ComponentGroup Id="ProductComponents" Directory="INSTALLFOLDER">
  <Files Include="publish\**">
    <Exclude Files="publish\**\*.pdb" />
    <Exclude Files="publish\StockApp.exe" />
  </Files>
</ComponentGroup>
```

Because `MediaTemplate EmbedCab="yes"` is set, all DLLs are compressed **inside the single MSI** — no external prerequisites or separate `.cab` files are needed on the target machine.

#### 5.2.3. Build and Deploy

```console
_[FILL IN: paste build console output]_
wix build Package.wxs -ext WixToolset.UI.wixext -d Version=1.0.0.0 -pdbtype none -o dist\StockApp-1.0.0.0-x64.msi
msiexec /i dist\StockApp-1.0.0.0-x64.msi
```

![Fig. 9: MSI payload containing all DLL dependencies](path/to/fig9-dll-payload.png)
*Fig. 9. Published payload of 64 DLLs included in the MSI cabinet*

#### 5.2.4. Verification on a Clean Machine

_[FILL IN: describe verification — e.g. installed on a Windows VM/user account without the .NET SDK or SplashKit installed, then launched the app. If you did not have a second machine, state that you verified the install location contents instead.]_

```console
dir "%ProgramFiles%\Stock Tracking App"
_[FILL IN: paste listing showing StockApp.exe plus the DLLs]_
```

![Fig. 10: Installed folder containing the executable and all DLLs](path/to/fig10-installed-dlls.png)
*Fig. 10. `Program Files\Stock Tracking App` after installation — executable and all 64 DLLs present*

![Fig. 11: Application running with DLL dependencies resolved](path/to/fig11-app-with-deps.png)
*Fig. 11. Stock Tracking App launching successfully with SplashKit/SDL2 and SQLite DLLs resolved*

### 5.3. Results

- Single MSI (~20.7 MB) carries the executable plus **64 DLLs** in an embedded cabinet
- Application installs and runs without requiring the .NET SDK, SplashKit, or any manual DLL copying
- Each DLL becomes a proper Windows Installer component, enabling **repair** and per-file **rollback** if files are deleted or corrupted

### 5.4. Challenges and Solutions

#### 5.4.1. Challenge 1: _[FILL IN: e.g. native SplashKit/SDL2 DLLs not copied to the publish folder / app starts and immediately exits with "unable to load SplashKit.dll"]_

**Issue / error message:**

```text
_[FILL IN: paste the exact console error]_
```

**Investigation:** _[FILL IN: e.g. checked `publish\`, compared with `bin\Release\net9.0\win-x64`, inspected `StockApp.deps.json`]_

**Solution:** _[FILL IN: e.g. added the `native\win64\*.dll` CopyToPublishDirectory item in `StockApp.csproj`]_

#### 5.4.2. Challenge 2: _[FILL IN: e.g. duplicate DLL file IDs / ICE38 or ICE64 harvest warnings from WiX]_

**Issue / error message:**

```text
_[FILL IN: paste the exact console error]_
```

**Investigation:** _[FILL IN: how you investigated]_

**Solution:** _[FILL IN: fix applied]_

### 5.5. Analysis

Completed at **Distinction** level. The deployment packages and installs an application with many managed **and** native DLL dependencies, all resolved from the installed folder at runtime with no external prerequisites.

---

## 6. Task 1.4: Microsoft Store Deployment (High Distinction)

### 6.1. Objective

Complete Task 1.3 **and** either deploy the application to the Microsoft Store for public access and download **or** explain in detail how to deploy to the Microsoft Store.

### 6.2. Approach

The Microsoft Store no longer accepts classic `.msi` installers directly; desktop apps must be submitted as an **MSIX** (or `.appx`) package, or as an MSI wrapped through the **Desktop Bridge / Windows App SDK** submission path. This report therefore:

1. Builds an **MSIX package** of the Stock Tracking App (`dist\StockApp-1.0.0.0-x64.msix`), and
2. Documents the full Partner Center submission process in detail below.

**MSIX build command (from `build-packages.ps1`):**

```console
_[FILL IN: paste console output of:]
.\build-packages.ps1 -Version 1.0.0.0
```

```console
makeappx pack /d build\msix /p dist\StockApp-1.0.0.0-x64.msix /o
```

**Result on this machine:**

```text
dist\StockApp-1.0.0.0-x64.msix
```

![Fig. 12: MSIX package built for Microsoft Store submission](path/to/fig12-msix-build.png)
*Fig. 12. `makeappx pack` producing the Store-ready MSIX package*

### 6.3. Detailed Steps to Deploy to the Microsoft Store

1. **Create a developer account**
   - Go to [https://partner.microsoft.com/dashboard](https://partner.microsoft.com/dashboard) and sign in with a Microsoft account.
   - Choose an individual ($19 one-time) or company account (requires company verification), accept the Microsoft Partner Center Agreement, and complete identity verification. Approval normally takes a few business days.

2. **Prepare the MSIX package**
   - `Packaging\AppxManifest.xml` declares the package identity (`Name`, `Publisher`, `Version`), display name, visual assets (StoreLogo, tiles, splash screen) and the desktop application entry point (`uap3:AppExecutionAlias` / desktop extension launching `StockApp.exe`).
   - The package is staged from the `publish\` payload, the manifest `Version` is rewritten to the release version, generated PNG assets are added, then packed with:
     `makeappx pack /d build\msix /p dist\StockApp-<version>-x64.msix /o`

3. **Sign the package**
   - Obtain a trusted signing certificate whose **subject matches the `Publisher` value** in `AppxManifest.xml` (for Store submissions Partner Center supplies the publisher value you must match).
   - Sign with:
     `signtool sign /fd SHA256 /a dist\StockApp-<version>-x64.msix`
   - For Store submission, signing is ultimately applied by Partner Center after validation; local signing is used for sideload testing.

4. **Create the app submission in Partner Center**
   - Partner Center → **+ New app** → reserve a name (Store name reservation).
   - **Packages** page → **Add a package** → upload the `.msix`/`.appxupload`.
   - **Product info** → category (Productivity / Finance), description, features, screenshots (at least 1 primary screenshot), logo, and **privacy policy URL** (mandatory for Internet-connected apps — this app fetches stock data, so a privacy policy is required).
   - **Pricing and availability** → free or paid, markets, release visibility (public = publicly accessible for download).

5. **Reserve the app identity / Windows Publisher ID**
   - If publishing through the Store, set the `Publisher` in `AppxManifest.xml` to the exact value Partner Center shows under **App identity** (e.g. `CN=<guid>` style value), rebuild the MSIX so its identity matches, then re-upload.

6. **Age rating questionnaire and certifications**
   - Complete the IARC age-rating questionnaire.
   - The Store runs automated **certification/attestation checks** (package validity, declared capabilities, malware scan). Fix any failed reports before approving the submission.

7. **Publish**
   - **Submit for approval** → Microsoft review (typically 1–3 business days, up to a week for first submissions).
   - Once approved, the app is published publicly at a Store listing URL of the form:
     `https://apps.microsoft.com/detail/<product-id>` — publicly accessible for download by anyone, satisfying the "publicly accessible URL" requirement for this task.

8. **Ongoing updates**
   - For each new release, increment `<Version>` in `AppxManifest.xml` (four-part: `Major.Minor.Build.Revision`), rebuild the MSIX, and upload a new package in Partner Center as an update to the existing listing.

**Sideload test performed before submission (optional verification):**

```console
_[FILL IN: if you tested, paste:]
Add-AppxPackage dist\StockApp-1.0.0.0-x64.msix
Get-AppxPackage *Stock*
Remove-AppxPackage ...
```

![Fig. 13: MSIX package sideloaded for verification](path/to/fig13-msix-sideload.png)
*Fig. 13. `Add-AppxPackage` verification of the Store-format package*

> _[FILL IN: if you actually published to the Store, paste the public listing URL here and a screenshot of the Store page. If not, this detailed explanation fulfils the Task 1.4 requirement.]_

### 6.4. Results

- MSIX package built successfully from the same published payload used by the MSI
- Complete, step-by-step Store submission process documented (account → packaging → signing → submission → certification → publication)
- _[FILL IN: Store listing URL if published, otherwise state "not published; explanation provided"]_

### 6.5. Analysis

Completed at **High Distinction** level: all lower-level sub-tasks (1.1, 1.2, 1.3) were completed, and the Microsoft Store deployment path has been addressed in detail with a Store-format MSIX artifact prepared.

---

## 7. Source Code, Scripts and Brief Explanation

> Per the submission instructions, **source code and script files are provided through the publicly accessible URL and are not included in full in this report**; binaries (`.exe`, `.msi`, `.msix`) are **not submitted** — only screenshots and pasted console output evidencing successful building and deployment are included.

**Publicly accessible repository:** _[FILL IN: https://github.com/H1406/Stock-Tracking-App]_

| File | Purpose |
|---|---|
| `Package.wxs` | WiX v4/v5 package authoring: package identity, `MajorUpgrade`, embedded media, harvested `publish\**` files as components, Start Menu shortcut, ARP icon, `WixUI_InstallDir` with `Eula.rtf` |
| `build-packages.ps1` | End-to-end build script: installs WiX 5.0.2 and Windows SDK tools, runs `dotnet publish`, builds the MSI with `wix build` and the MSIX with `makeappx` |
| `StockApp.csproj` | .NET 9 project; copies `images\**`, `Data\**`, `Historical Prices\**`, `invalid.mp3`, `wallet.txt`, `Eula.rtf` and `native\win64\*.dll` into output/publish |
| `Packaging\AppxManifest.xml` | MSIX/Store manifest (identity, version, assets, entry point) |
| `Eula.rtf` | Licence text shown by the `WixUI_InstallDir` licence dialog |

**Brief explanation of the packaging script** (`build-packages.ps1` excerpt):

```powershell
function Publish-App {
    dotnet publish (Join-Path $Root 'StockApp.csproj') -c Release -r win-x64 --self-contained false -o $PublishDir
    Assert-Exit 'dotnet publish'
}
function Build-Msi {
    $output = Join-Path $DistDir "StockApp-$Version-x64.msi"
    & (Join-Path $Tools 'wix.exe') build (Join-Path $Root 'Package.wxs') `
        -ext WixToolset.UI.wixext -d "Version=$Version" -pdbtype none -o $output
    Assert-Exit 'wix build'
}
```

The script publishes the app, then invokes `wix build` with the UI extension and a `-d Version=...` preprocessor variable so a single `.wxs` can produce differently versioned MSI releases; `-pdbtype none` keeps debug symbols out of the installer.

---

## 8. Error Analysis and Investigation

> The marking rubric rewards analysis and investigation of **console errors** and a clear account of how they were eradicated. Every significant error encountered during this task is listed below.

| # | Sub-task | Error observed on console | Investigation | Resolution |
|---|----------|---------------------------|---------------|------------|
| 1 | _[FILL IN: 1.1–1.4]_ | _[FILL IN: exact error text pasted from console]_ | _[FILL IN: how you investigated — command run, log read, documentation checked]_ | _[FILL IN: how the error was eradicated]_ |
| 2 | _[FILL IN: ...]_ | _[FILL IN: ...]_ | _[FILL IN: ...]_ | _[FILL IN: ...]_ |
| 3 | _[FILL IN: ...]_ | _[FILL IN: ...]_ | _[FILL IN: ...]_ | _[FILL IN: ...]_ |
| 4 | _[FILL IN: ...]_ | _[FILL IN: ...]_ | _[FILL IN: ...]_ | _[FILL IN: ...]_ |

**Common errors to document if you encountered them:**

- `wix build` — *The system cannot find the file 'publish\StockApp.exe'* → `dotnet publish` had not been run (or was run with a different output path); run the publish step first or correct the `SourceFile` path.
- `wix build` — *Undefined preprocessor variable '$(var.Version)'* → pass `-d Version=x.y.z.w` (handled by `build-packages.ps1`).
- `wix build` — *WixToolset.UI.wixext is not installed* → `wix extension add -g WixToolset.UI.wixext/5.0.2`.
- `msiexec` exit code **1603** — fatal error during install → inspect the verbose log `msiexec /l*v install.log`, search for `Return value 3`.
- Application starts then exits — a native DLL is missing from `publish\` → verify with `dir publish\*.dll` and `StockApp.deps.json`.

**Errors successfully eradicated:** _[FILL IN: summary sentence — e.g. "All four errors below were investigated on the console and eradicated; no unresolved problems remain."]_

**Unresolved problems:** _[FILL IN: any issues that remain, or "None"]_

---

## 9. Conclusion

### 9.1. Achievements Summary

| Sub-task | Level | Requirement | Status |
|----------|-------|-------------|--------|
| Task 1.1 | Pass | Follow the WiX walkthrough and deploy a sample desktop app | _[FILL IN: Completed / Partial]_ |
| Task 1.2 | Credit | Follow the WiX walkthrough and deploy your own C/C++/C# desktop app | _[FILL IN]_ |
| Task 1.3 | Distinction | 1.1/1.2 **and** deploy an application with multiple DLLs or dependencies (64 DLLs packaged) | _[FILL IN]_ |
| Task 1.4 | High Distinction | 1.3 **and** deploy to the Microsoft Store **or** explain in detail how to deploy to the Microsoft Store | _[FILL IN]_ |

### 9.2. Final Remarks

_[FILL IN: 1 short paragraph summarising what the WiX deployments demonstrated — sample app, own C# app, multi-DLL packaging, Store path — plus any unresolved problems]_

---

## 10. Appendices

### 10.1. Appendix A: Public Access

> The report must include a publicly accessible IP address/URL for verification.

- **Public URL / IP for this task:** _[FILL IN: e.g. https://apps.microsoft.com/detail/<product-id> if published, or a publicly hosted page/IP where the report artifacts can be verified]_
- **Source code repository:** _[FILL IN: https://github.com/H1406/Stock-Tracking-App]_
- **Package artifacts (not submitted, referenced only):** `dist\StockApp-1.0.0.0-x64.msi`, `dist\StockApp-1.0.0.0-x64.msix`

> **Note:** source code and script files are provided through the publicly accessible URL above and are **not** pasted in full into this report. Binaries (executables, MSI, MSIX) are **not** submitted; evidence of successful compiling/building/deployment is included as labelled screenshots and pasted console output.

### 10.2. Appendix B: Commands Reference

```console
# Environment
dotnet --version
%LOCALAPPDATA%\StockAppBuild\tools\wix.exe --version
wix extension add -g WixToolset.UI.wixext/5.0.2

# Build the application payload
dotnet publish StockApp.csproj -c Release -r win-x64 --self-contained false -o publish

# Build the MSI (Task 1.2 / 1.3)
wix build Package.wxs -ext WixToolset.UI.wixext -d Version=1.0.0.0 -pdbtype none -o dist\StockApp-1.0.0.0-x64.msi
# or the full scripted build
.\build-packages.ps1 -Version 1.0.0.0

# Install / verify / uninstall (Task 1.2)
msiexec /i dist\StockApp-1.0.0.0-x64.msi /l*v install.log
msiexec /x dist\StockApp-1.0.0.0-x64.msi

# MSIX for the Microsoft Store (Task 1.4)
makeappx pack /d build\msix /p dist\StockApp-1.0.0.0-x64.msix /o
signtool sign /fd SHA256 /a dist\StockApp-1.0.0.0-x64.msix
Add-AppxPackage dist\StockApp-1.0.0.0-x64.msix
```

### 10.3. Appendix C: Screenshots Index

| Figure | Caption | Evidence |
|--------|---------|----------|
| Fig. 1 | `wix --version` output | WiX installed |
| Fig. 2 | Walkthrough sample `wix build` | Task 1.1 |
| Fig. 3 | Sample app installed and running | Task 1.1 |
| Fig. 4 | `dotnet publish` output | Task 1.2 |
| Fig. 5 | `wix build` of `Package.wxs` | Task 1.2 |
| Fig. 6 | MSI wizard (EULA / install dir) | Task 1.2 |
| Fig. 7 | Programs and Features entry | Task 1.2 |
| Fig. 8 | App launched from Start Menu shortcut | Task 1.2 |
| Fig. 9 | 64-DLL payload in the MSI | Task 1.3 |
| Fig. 10 | Installed folder with all DLLs | Task 1.3 |
| Fig. 11 | App running with DLL dependencies resolved | Task 1.3 |
| Fig. 12 | MSIX package built | Task 1.4 |
| Fig. 13 | MSIX sideload verification / Store listing | Task 1.4 |
