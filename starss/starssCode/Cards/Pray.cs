// 【已停用 2026-11-03】祈福（Pray）按需求移出卡池。
// 恢复步骤：删掉本文件每行行首的 "// "，并在 localization/eng/cards.json 补回
//           STARSS-PRAY.title / .description 两条（缺一会触发 STS001 编译错误）。
// using MegaCrit.Sts2.Core.Commands;
// using MegaCrit.Sts2.Core.Entities.Cards;
// using MegaCrit.Sts2.Core.GameActions.Multiplayer;
// using MegaCrit.Sts2.Core.Localization.DynamicVars;
// using starss.starssCode.Powers;
// using System.Collections.Generic;
// using System.Threading.Tasks;
//
// namespace starss.starssCode.Cards;
//
// public sealed class Pray : starssCard
// {
//     public Pray()
//         : base(2, CardType.Power, CardRarity.Rare, TargetType.Self)
//     {
//     }
//
//     protected override IEnumerable<DynamicVar> CanonicalVars =>
//     [
//         new DynamicVar("Power", 7M)
//     ];
//
//     protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
//     {
//         await PowerCmd.Apply<PrayPower>(
//             choiceContext,
//             Owner.Creature,
//             DynamicVars["Power"].BaseValue,
//             Owner.Creature,
//             this
//         );
//     }
//
//     protected override void OnUpgrade()
//     {
//         DynamicVars["Power"].UpgradeValueBy(3M);
//     }
// }