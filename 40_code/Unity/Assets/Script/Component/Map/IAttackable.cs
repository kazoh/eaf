
public interface IAttackable {

    void Attacked(UnityEngine.Vector3 _pos, int _atk, bool _critical, bool _knockback);
    bool IsDie();

}
