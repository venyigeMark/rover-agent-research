using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class MovementTests
{
    private GameObject testRover;
    private MovementController controller;

    [SetUp]
    public void Setup()
    {
        // Tesztkörnyezet felépítése
        testRover = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        // Rigidbody hozzáadása, de a GRAVITÁCIÓ KIKAPCSOLÁSA a teszthez!
        Rigidbody rb = testRover.AddComponent<Rigidbody>();
        rb.useGravity = false;

        controller = testRover.AddComponent<MovementController>();
        controller.speed = 5f;
    }

    [TearDown]
    public void Teardown()
    {
        // Takarítás a teszt után
        Object.Destroy(testRover);
    }

    [UnityTest]
    public IEnumerator Rover_ResetsToStartPosition()
    {
        // Kezdeti pozíció lekérése (0,0,0)
        Vector3 startPos = testRover.transform.position;

        // Szimuláljuk, hogy a rover elmozdult (pl. 10 métert ment)
        testRover.transform.position = new Vector3(10, 0, 10);
        yield return new WaitForFixedUpdate();

        // Reset funkció meghívása
        controller.ResetPosition();
        yield return new WaitForFixedUpdate();

        // Ellenõrzés: A pozíció megegyezik-e a kezdõpozícióval
        Assert.AreEqual(startPos, testRover.transform.position);
    }
}