/// O script precisa ser adicionado diretamente a moto
public float CurrentLeanAngle => Vector3.SignedAngle(Vector3.up, transform.up, transform.forward);
