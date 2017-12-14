using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticInfo  {



	public static float[] getChallengeTimes(WorldEntrance.World world) {
        switch (world) {
            case WorldEntrance.World.Forest:
                return ForestTimes();
            case WorldEntrance.World.Desert:
                return DesertTimes();
            case WorldEntrance.World.Canyon:
                return CanyonTimes();
            case WorldEntrance.World.Island:
                return IslandTimes();
            case WorldEntrance.World.Space:
                return SpaceTimes();
            default:
                Debug.LogError("Could not get challenge times.");
                return null;
        }
    }

    private static float[] ForestTimes() {
        float[] times = new float[10];

        times[0] = 10f; //4.85
        times[1] = 18f; //9.85
        times[2] = 30f; //8.18
        times[3] = 20f; //9.57
        times[4] = 30f; //12.86
        times[5] = 8f; //5.59
        times[6] = 20f; //14.87
        times[7] = 10f; //7.40
        times[8] = 20f; //6.49
        times[9] = 50f; //34.41

        return times;
    }

    private static float[] DesertTimes() {
        float[] times = new float[10];

        times[0] = 15f; //9.85
        times[1] = 30f; //18.72
        times[2] = 20f; //8.60
        times[3] = 8f; //5.01
        times[4] = 25f; //16.81
        times[5] = 15f; //6.89
        times[6] = 5f; //1.93
        times[7] = 12f; //8.51
        times[8] = 20f; //3.42
        times[9] = 60f; //39.46

        return times;
    }

    private static float[] CanyonTimes() {
        float[] times = new float[10];

        times[0] = 5f; //3.96
        times[1] = 22f; //13.63
        times[2] = 10f; //6.15
        times[3] = 65f; //60.41
        times[4] = 90f; //85.30
        times[5] = 5f; //3.49
        times[6] = 28f; //22.58
        times[7] = 45f; //40.79
        times[8] = 85f; //78.63
        times[9] = 25f; //22.91

        return times;
    }
    private static float[] IslandTimes() {
        float[] times = new float[10];

        times[0] = 8f; //4.93
        times[1] = 20f; //15.67
        times[2] = 20f; //13.80
        times[3] = 40f; //27.30
        times[4] = 83f; //79.76
        times[5] = 15f; //11.59
        times[6] = 10f; //4.22
        times[7] = 20f; //6.30
        times[8] = 55f; //45.29
        times[9] = 60f; //51.76

        return times;
    }
    private static float[] SpaceTimes() {
        float[] times = new float[10];

        times[0] = 33f; //22.64
        times[1] = 35f; //25.01
        times[2] = 40f; //22.41
        times[3] = 22f; //13.03
        times[4] = 18f; //8.92
        times[5] = 40f; //33.54
        times[6] = 66f; //63.66
        times[7] = 53f; //47.23
        times[8] = 25f; //13.08
        times[9] = 40f; //31.78

        return times;
    }
}
