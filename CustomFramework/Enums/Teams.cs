using System;

namespace CustomFramework.Enums
{
    [Flags]
    public enum Teams
    {
        None = 0,
        ClassD = 1 << 0,
        Science = 1 << 1,
        FacilityGuard = 1 << 2,
        MobileTaskForce = MTFCaptain | MTFSergeant | MTFPrivate,
        MTFCaptain = 1 << 3,
        MTFSergeant = 1 << 4,
        MTFSpecialist = 1 << 5,
        MTFPrivate = 1 << 6,
        ChaosInsurgency = CIRepressor | CIMarauder | CIRifleman,
        CIRepressor = 1 << 7,
        CIMarauder = 1 << 8,
        CIRifleman = 1 << 9,
        CIConscript = 1 << 10,
        SCP = SCP049 | SCPZombie | SCP079 | SCP096 | SCP106 | SCP173 | SCP939 | SCP3114,
        SCPZombie = SCP0492 | SCP1507Zombie,
        SCP049 = 1 << 11,
        SCP0492 = 1 << 12,
        SCP079 = 1 << 13,
        SCP096 = 1 << 14,
        SCP106 = 1 << 15,
        SCP173 = 1 << 16,
        SCP939 = 1 << 17,
        SCPFlamingo = SCP1507Alpha | SCP1507,
        SCP1507Alpha = 1 << 18,
        SCP1507 = 1 << 19,
        SCP1507Zombie = 1 << 20,
        SCP3114 = 1 << 21,
    }
}
