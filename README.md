<p align="center">
  <img src="icon.png" alt="Elegant Melee Logo" width="128" height="128" />
</p>

<h1 align="center">MoreElegance - Elegant Melee Mod</h1>

<p align="center">
  <em>Ascendendo a elegância marcial no Terraria com esgrima refinada, estocadas de alta precisão e arquitetura aristocrática.</em>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/tModLoader-v1.4.4%20(2023.x%2B)-2ea44f?style=for-the-badge&logo=terraria" alt="tModLoader 1.4.4" />
  <img src="https://img.shields.io/badge/.NET-6.0-512BD4?style=for-the-badge&logo=dotnet" alt=".NET 6.0" />
  <img src="https://img.shields.io/badge/C%23-10%20%2F%2011-239120?style=for-the-badge&logo=csharp" alt="C#" />
  <img src="https://img.shields.io/badge/Calamity%20Mod-Weak%20Reference-00b4d8?style=for-the-badge" alt="Calamity Ready" />
  <img src="https://img.shields.io/badge/License-MIT-yellow?style=for-the-badge" alt="License MIT" />
</p>

---

## 📖 Visão Geral

**MoreElegance (Elegant Melee)** é uma modificação para o **tModLoader 1.4.4** focada em transformar o combate corpo-a-corpo tradicional em uma experiência de alta precisão técnica e elegância aristocrática. 

O mod introduz a subclasse **Elegant Melee**, armas de estocada (rapieiras) controladas em tempo real pela mira do cursor, um NPC mentor especializado, uma arena de treinamento indestrutível, compatibilidade opcional e fluida com o **Calamity Mod**, e um evento de reconstrução mundial procedural no endgame.

---

## ⚔️ Funcionalidades Principais

### 1. Subclasse Customizada (`ElegantMeleeDamageClass`)
- Herda **100% dos modificadores** da classe Melee tradicional (`DamageClass.Melee` e `DamageClass.Generic`): dano, chance de crítico, velocidade de ataque, penetração de armadura e knockback.
- Herança total de **efeitos de combate corpo-a-corpo**, garantindo compatibilidade nativa com frascos/imbuições de armas (Flasks), pedras de magma e acessórios de autoswing.

### 2. Rapieiras & Projéteis de Estocada (`RapierBase` & `ProjRapierBase`)
- **Rapier Base**: Arma inicial de esgrima causando 12 de dano elegante com animação ultrarrápida (`useTime: 15`).
- **Mecânica de Estocada Direcional**: O projétil acompanha a posição do cursor do jogador com avanço e retração elásticos e precisos (`MathHelper.SmoothStep`), com sistema de imunidade local para acertos limpos, faíscas prateadas e som metálico de impacto.
- **Receita**: 4 Barras de Ferro/Chumbo + 1 Madeira Boreal em uma Bigorna.

### 3. Bloco de Treinamento Indestrutível (`TrainingTile` & `TrainingBlock`)
- Bloco reforçado e impenetrável projetado para salas de duelo e treino.
- **Resistência Absoluta**: `MinPick = 9999`, `Main.tileSolid = true`, `Main.tileNoFail = true` e imunidade total a explosivos (`CanExplode = false`) e mineração (`CanKillTile = false`).
- Acompanha o item colocável `TrainingBlock`.

### 4. O General NPC (`GeneralNPC`)
- **Lore**: Um distinto mestre espadachim idoso com bigode aristocrático impecável e sobretudo militar de viagem, inspirado em **Spring Mustachio** (*One Punch Man*) e no **Traveling Merchant**.
- **Condição de Spawn**: Muda-se para uma casa vaga assim que o jogador possui qualquer arma corpo-a-corpo no inventário.
- **Teleporte para o Treinamento**: Botão no chat *"Go to Training"* que teletransporta o jogador instantaneamente para as coordenadas seguras da câmara de treinamento (`X = 50 * 16, Y = 200 * 16`), gerando a arena de `TrainingTile` caso ainda não exista.
- **Defesa Própria**: Desfere ataques ágeis de rapieira para proteger a vila de invasores.

### 5. Integração com Calamity Mod (`AerialiteRapier`)
- Utiliza **referência fraca** (`weakReferences = CalamityMod` no `build.txt`) via `ModLoader.TryFind`.
- **Rapieira de Aerialite**: Arma veloz que dispara pulsos de vento cortante no ápice da estocada.
- **Detecção Dinâmica**:
  - Com o Calamity Mod ativo: A receita de criação (Barras de Aerialite, Blocos de Placa Solar, Penas e Rapier Base) é habilitada automaticamente.
  - Sem o Calamity Mod: A receita não é carregada, mantendo o item seguro na montagem sem causar erros ou travamentos (acessível apenas via Cheat Sheet).

### 6. Evento de Reconstrução Mundial Endgame (`WorldUpdateSystem`)
- **Condições de Ativação**:
  1. `Main.hardMode && NPC.downedMoonlord` (Moon Lord derrotado).
  2. `WorldGen.corruptTiles <= 0 && WorldGen.crimsonTiles <= 0` (Mundo 100% purificado de Corrupção e Carmesim).
- **Sequência de Transformação**:
  - O General desbloqueia o botão e diálogo especial *"Reconstruct Realm"*.
  - Limpa estruturas de madeira provisórias no ponto de spawn da Floresta.
  - Constrói proceduralmente o **Grande Palácio Aristocrático da Elegância**: fundação de blocos indestrutíveis, colunas de mármore clássicas, teto abobadado em tijolos dourados, vitrais de vidro, lustres nobres, bonecos de treino para esgrima e mobília imperial.
  - Realoca os NPCs com segurança no interior do palácio e persiste o estado no save via `TagCompound`.

---

## 📂 Estrutura do Repositório

```
MoreElegance/
├── build.txt                                           # Metadados e dependências fracas do tModLoader
├── description.txt                                     # Descrição exibida no Mod Browser in-game
├── MoreElegance.csproj                                 # Configuração de compilação em .NET 6
├── MoreElegance.cs                                     # Classe principal do mod
├── icon.png                                            # Ícone oficial em pixel art (80x80)
│
├── Common/
│   ├── DamageClasses/
│   │   └── ElegantMeleeDamageClass.cs                 # Subclasse de dano com herança Melee
│   └── Systems/
│       └── WorldUpdateSystem.cs                       # Verificação de purificação e WorldGen do Palácio
│
├── Content/
│   ├── Items/
│   │   ├── Placeables/
│   │   │   ├── TrainingBlock.cs                       # Item do bloco de treinamento
│   │   │   └── TrainingBlock.png
│   │   └── Weapons/
│   │       ├── RapierBase.cs                          # Rapieira de treino inicial
│   │       ├── RapierBase.png
│   │       ├── AerialiteRapier.cs                     # Rapieira de Aerialite (Calamity)
│   │       └── AerialiteRapier.png
│   │
│   ├── NPCs/
│   │   ├── GeneralNPC.cs                              # The General (Town NPC com teleporte)
│   │   ├── GeneralNPC.png                             # Sprite sheet com 25 frames
│   │   └── GeneralNPC_Head.png                        # Ícone para o mapa
│   │
│   ├── Projectiles/
│   │   └── Weapons/
│   │       ├── ProjRapierBase.cs                      # Projétil de estocada com mira no cursor
│   │       ├── ProjRapierBase.png
│   │       ├── ProjAerialiteRapier.cs                 # Projétil de estocada de vento
│   │       └── ProjAerialiteRapier.png
│   │
│   └── Tiles/
│       ├── TrainingTile.cs                            # Bloco indestrutível (MinPick 9999)
│       └── TrainingTile.png                           # Folha de sprites do tile
│
└── Localization/
    └── en-US_Mods.MoreElegance.hjson                  # Arquivo moderno de localização HJSON
```

---

## 🛠️ Como Instalar e Compilar

### Opção 1: Diretório ModSources do tModLoader
1. Clone ou copie esta pasta para o diretório de mods locais:
   - **Windows**: `%USERPROFILE%\Documents\My Games\Terraria\tModLoader\ModSources\MoreElegance`
   - **Linux**: `~/.local/share/Terraria/tModLoader/ModSources/MoreElegance`
2. Abra o **tModLoader 1.4.4**.
3. Navegue até **Workshop > Develop Mods (Desenvolver Mods)**.
4. Localize **MoreElegance / Elegant Melee** e clique em **Build & Reload (Compilar e Recarregar)**.

### Opção 2: Linha de Comando (.NET CLI)
```bash
# Compilar projeto com o SDK do .NET 6
dotnet build MoreElegance.csproj -c Release
```

---

## 📜 Licença

Distribuído sob a licença [MIT](LICENSE). Consulte o arquivo `LICENSE` para mais detalhes.
