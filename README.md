# EEA 프로젝트

## 📝 프로젝트 소개

Unity 기반의 2D 액션 RPG 스타일 게임 프로젝트입니다. 다양한 스킬, 오브젝트, 아이템, 네트워크, UI 시스템과 Addressable Asset, 커스텀 에디터 툴을 활용하여 확장성과 유지보수성을 높였습니다.

---

## 📁 폴더 구조

```
├── Assets
│   ├── Scripts         # 게임 로직(C#)
│   │   ├── Ability     # 스킬/능력치 시스템
│   │   ├── Manager     # 게임/씬/UI/리소스/테이블 매니저
│   │   ├── Object      # 플레이어, 적, 아이템 등 오브젝트
│   │   ├── Scene       # 씬 상태 및 전환 관리
│   │   ├── UI          # UI 시스템 및 컴포넌트
│   │   ├── Protocols   # 네트워크/데이터 통신
│   │   ├── Utils       # 유틸리티 함수 및 상수
│   │   └── ...
│   ├── AddressableAssets # Addressable 리소스(프리팹, UI, 사운드 등)
│   ├── Editor          # 커스텀 에디터 및 데이터 변환 툴
│   └── ...
├── Packages            # Unity 패키지 의존성
├── ProjectSettings     # Unity 프로젝트 설정
└── ...
```

---

## 🚀 주요 기능

- **스킬/능력치 시스템**: 다양한 스킬(발사체, 오브, 레이저, 메테오 등)과 능력치 관리
- **오브젝트 시스템**: 플레이어, 근거리/원거리 적, 드랍 아이템 등
- **매니저 시스템**: GameManager, UIManager, SceneManager, TableManager 등 싱글톤 기반 관리
- **UI 시스템**: Addressable 기반의 동적 UI 생성, 인벤토리, HUD, 팝업 등
- **네트워크/프로토콜**: DTO, 프로토콜, 네트워크 매니저 구조화
- **Addressable Asset**: 리소스 효율적 관리 및 동적 로딩
- **커스텀 에디터 툴**: Excel → Json/Binary 변환, CDN, Server 업로드 등 지원

---

## 🛠️ 실행 방법

1. **Unity Hub에서 프로젝트 열기**
   - Unity 6000
2. **패키지 설치**
   - `Packages/manifest.json` 및 `Packages/packages-lock.json`에 정의된 패키지 자동 설치
   - 필요시 Unity Package Manager에서 Addressables, Cinemachine 등 수동 설치
3. **Addressables 빌드**
   - 메뉴: `Window > Asset Management > Addressables > Groups` → Build
4. **게임 실행**
   - `GameInitializer`가 포함된 씬(Intro)에서 Play
   - 기본 진입 씬: IntroScene → LobbyScene → StageScene

---

## 📦 주요 의존성

- Unity 2D Animation, Tilemap
- Addressables, Cinemachine, InputSystem, PostProcessing
- Newtonsoft.Json, VisualEffectGraph, ShaderGraph 등
- DOTween(플러그인)

---

## 💡 개발 참고사항

- **커스텀 에디터**: `Assets/Editor/ExcelParser.cs`에서 Excel 데이터 변환 및 CDN 업로드 지원
- **Addressable 리소스**: `Assets/AddressableAssets` 하위에 프리팹, UI, 사운드 등 관리
- **씬 관리**: Addressable로 씬을 관리하며, SceneManager에서 상태 전환
- **코드 스타일**: 싱글톤 패턴, 코루틴, 인터페이스, 체이닝 등 적극 활용
- **.gitignore**: 빌드/라이브러리/임시/메타/에셋 파일 등은 버전 관리에서 제외

---
