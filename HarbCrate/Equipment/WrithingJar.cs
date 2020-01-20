﻿using R2API;
using RoR2;
using UnityEngine;

namespace HarbCrate.Equipment
{
    [Equipment]
    internal sealed class WrithingJar : Equip
    {
        public WrithingJar() : base()
        {

            Name = new TokenValue("HC_WORMJAR", "The Writhing Jar");
            Description = new TokenValue("HC_WORMJAR_DESC",
                "Summons a <style=cHealing>friendly</style> Magma Worm and a <style=cDeath>HOSTILE</cstyle> Overloading Magma Worm.");
            PickupText = new TokenValue("HC_WORMJAR_DESC",
                "Inside of you there's two worms. One is a friend, the other a monster. You are a <color=blue>monster</color>.");
            AssetPath = HarbCratePlugin.assetPrefix+"Assets/HarbCrate/2_Worm_Jar/WrithingJar.prefab";
            SpritePath = "";
            Cooldown = 120f;
            IsLunar = true;
            IsEnigmaCompat = true;
            
            hostileCard = Resources.Load<SpawnCard>("SpawnCards/CharacterSpawnCards/cscElectricWorm");
            friendCard = Resources.Load<SpawnCard>("SpawnCards/CharacterSpawnCards/cscMagmaWorm"); 
        }

        private readonly SpawnCard hostileCard;
        private readonly SpawnCard friendCard;
        
        private const float HostileDMG = 3;
        private const float HostileHP = 4.7f;
        private const float AllyDMG = 1.5f;
        private const float AllyHP = 1;

        public override bool Effect(EquipmentSlot equipmentSlot)
        {
            var transform = equipmentSlot.transform;
            var placementRules = new DirectorPlacementRule
            {
                placementMode = DirectorPlacementRule.PlacementMode.Approximate,
                minDistance = 10f,
                maxDistance = 100f,
                spawnOnTarget = transform
            };
            var hateRequest = new DirectorSpawnRequest(hostileCard, placementRules, RoR2Application.rng)
            {
                ignoreTeamMemberLimit = false,
                teamIndexOverride = TeamIndex.Monster
            };
            var spawn = DirectorCore.instance.TrySpawnObject(hateRequest);
            spawn.transform.TransformDirection(0, 100, 0);
            if (spawn)
            {
                CharacterMaster cm = spawn.GetComponent<CharacterMaster>();
                if (cm)
                {
                    cm.inventory.GiveItem(ItemIndex.BoostDamage, getItemCountFromMultiplier(HostileDMG));
                    cm.inventory.GiveItem(ItemIndex.BoostHp, getItemCountFromMultiplier((HostileHP)));
                }
            }
            var friendRequest = new DirectorSpawnRequest(friendCard, placementRules, RoR2Application.rng)
            {
                ignoreTeamMemberLimit = false,
                teamIndexOverride = TeamIndex.Player,
                summonerBodyObject = equipmentSlot.GetComponent<CharacterBody>().gameObject
            };
            var spawn2 = DirectorCore.instance.TrySpawnObject(friendRequest);
            spawn2.transform.TransformDirection(0, 100, 0);
            if (spawn2)
            {
                CharacterMaster cm = spawn2.GetComponent<CharacterMaster>();
                if (cm)
                {
                    cm.inventory.GiveItem(ItemIndex.BoostDamage, getItemCountFromMultiplier(AllyDMG));
                    cm.inventory.GiveItem(ItemIndex.BoostHp, getItemCountFromMultiplier((AllyHP)));
                }   
            }
            if (spawn || spawn2)
                return true;
            return false;
        }


        private int getItemCountFromMultiplier(float multiplier)
        {
            if (multiplier <= 1)
                return 0;
            return Mathf.CeilToInt((multiplier - 1) * 10);
        }

        public override void Hook()
        { }
    }
}
