using UnityEngine;

public class AttackState_Melee : EnemyState
{
    private Enemy_Melee enemy;
    private Vector3 attackDirection;
    private float attackMoveSpeed;

    private Vector3 dashTarget;
    private float dashTimer;

    private float shootTick;

    private float flameTick;

    private float damageIncrement;
    private float attackSpeedIncrement;

    private const float MAX_ATTACK_DISTANCE = 50f;

    public AttackState_Melee(EnemyBase enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Melee;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.PullWeapon();
        //Vide anotação na script Enemy_Melee!!

        SelectAttackBasedOnDistance();

        // Evita briga do agent com movimento manual
        if (!enemy.attackData.useAgentDuring)
        {
            enemy.agent.isStopped = true;
            enemy.agent.updatePosition = false;
            enemy.agent.updateRotation = false;
            enemy.agent.enabled = false;
        }
    }

    public override void Exit()
    {
        base.Exit();

    }



    public override void Update()
    {
        base.Update();

        if (enemy.attackData.rotateDuring)
            enemy.transform.rotation = enemy.FaceTarget(enemy.player.position);

        switch (enemy.attackData.attackType)
        {
            case AttackType_Melee.Charge:
                DoDash();
                break;

            case AttackType_Melee.Shoot:
                DoShootChase();
                break;

            case AttackType_Melee.Flamethrower:
                DoFlameBand();
                break;

            case AttackType_Melee.Close:
                // se quiser um “soco”/empurrão, você pode tratar aqui parecido com Charge curto
                stateMachine.ChangeState(enemy.recoveryState);
                break;
        }

    }

    private void DoDash()
    {
        if (!enemy.enemyScript.isDashing)
        {
            enemy.enemyScript.isDashing = true;
            enemy.enemyScript.wasDashDamageApplied = false;

            // trava a direção no momento que iniciar o Dash
            Vector3 forward = enemy.transform.forward;

            
            //Faz com que o inimigo use o Dash para se afastar
            if (enemy.attackData.moveSpeed < 0)
            {
                forward *= -1f;
            }

            dashTarget = enemy.transform.position + forward.normalized * Mathf.Max(1f, enemy.attackData.dashDistance);
            
            //if(enemy.attackData.moveSpeed < 0)
            //{
            //    dashTarget = enemy.transform.position - forward.normalized * Mathf.Max(1f, enemy.attackData.dashDistance);
            //}


            // se preferir tempo em vez de distância:
            dashTimer = Mathf.Max(0.01f, enemy.attackData.dashDuration);
        }

        // movimento manual em linha reta
        float spd = (enemy.attackData.dashDuration > 0f)
                    ? (enemy.attackData.dashDistance / enemy.attackData.dashDuration)
                    : Mathf.Max(0.01f, enemy.attackData.moveSpeed);

        enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, dashTarget, spd * Time.deltaTime);

        // não gira durante o dash (rotateDuring=false no Charge)

        // fim por distância OU por tempo
        bool reached = Vector3.Distance(enemy.transform.position, dashTarget) < 0.05f;
        dashTimer -= Time.deltaTime;

        if (reached || dashTimer <= 0f)
        {
            enemy.recoveryTime = enemy.attackData.recoveryTime;
            stateMachine.ChangeState(enemy.recoveryState);
            Debug.Log("Fui chamado após o dash para entrar no recovery");
        }
    }

    private void DoShootChase()
    {
        // Garante agent ligado
        if (!enemy.agent.enabled)
        {
            enemy.agent.enabled = true;
            enemy.agent.isStopped = false;
            enemy.agent.updateRotation = true;
            //enemy.agent.speed = enemy.attackData.moveSpeed;
        }

        float d = Vector3.Distance(enemy.transform.position, enemy.player.position);

        // Controla a distância:
        if (d > enemy.attackData.preferMax)
        {
            // Aproxima
            enemy.agent.speed = enemy.attackData.moveSpeed;
            enemy.agent.stoppingDistance = Mathf.Clamp(enemy.attackData.preferMax - 0.2f, 0f, 100f);
            enemy.agent.updateRotation = true;
            enemy.agent.destination = enemy.player.position;
        }
        else if (d < enemy.attackData.preferMin)
        {
            // Afasta: destino oposto ao player
            Vector3 dir = (enemy.transform.position - enemy.player.position).normalized;

            enemy.agent.updateRotation = false;
            enemy.agent.speed = enemy.attackData.moveSpeed * .5f;

            Vector3 away = enemy.transform.position + dir * (enemy.attackData.preferMin + 1.5f);
            enemy.agent.stoppingDistance = 0f;
            enemy.agent.destination = away;
        }
        else
        {
            // Dentro da faixa — orbitar/ajustar pouco (opcional: caminhar lateralmente)
            enemy.agent.stoppingDistance = (enemy.attackData.preferMin + enemy.attackData.preferMax) * 0.5f;
            enemy.agent.destination = enemy.player.position;
        }

        // Disparo por cadência
        shootTick -= Time.deltaTime;
        if (shootTick <= 0f)
        {
            enemy.enemyScript?.Fire(); // usa o sistema atual de projéteis que está no EnemyScript
            //shootTick = Mathf.Max(0.05f, enemy.attackData.attackSpeed);
            shootTick = enemy.attackData.attackSpeed;
            //O cálculo do cooldown está sendo feito aqui, eu retirei do enemyscript temporariamente
            //Ver onde fica melhor para buffar o inimigo posteriormente
        }

        if (enemy.enemyScript.useZoneMissileDuringShoot)
        {
            enemy.enemyScript.TryStartZoneMissileAttack();
        }

        // Dasher: míssil guiado enquanto está no ataque de tiro
        if (enemy.enemyScript.useGuidedMissileDuringShoot)
        {
            enemy.enemyScript.TryShootGuidedMissile();
        }

        // Condição de saída do estado de ataque:
        // Se o player saiu da faixa geral desse ataque, reavalia (recovery rápido ajuda o “swap” de ataque)
        float min = enemy.attackData.attackMinRange;
        float max = enemy.attackData.attackMaxRange;
        if (d < min - 0.5f || d > max + 0.5f)
            stateMachine.ChangeState(enemy.recoveryState);
    }

    private void DoFlameBand()
    {
        if (!enemy.agent.enabled)
        {

            enemy.agent.enabled = true;
            enemy.agent.isStopped = false;
            enemy.agent.updateRotation = true;
            enemy.agent.speed = enemy.attackData.moveSpeed;
        }



        float d = Vector3.Distance(enemy.transform.position, enemy.player.position);

        // Liga/desliga partículas
        bool inBand =  d <= enemy.attackData.preferMax; //d >= enemy.attackData.preferMin &&
        if (enemy.flameControl) enemy.flameControl.SetActive(inBand);

        // Mover para manter banda
        if (d > enemy.attackData.preferMax)
        {
            enemy.agent.stoppingDistance = enemy.attackData.preferMax;
            enemy.agent.destination = enemy.player.position;
        }
        else if (d < enemy.attackData.preferMin)
        {
            Vector3 dir = (enemy.transform.position - enemy.player.position).normalized;
            Vector3 away = enemy.transform.position + dir * (enemy.attackData.preferMin + 1.0f);
            enemy.agent.updateRotation = false;
            enemy.agent.stoppingDistance = 0f;
            enemy.agent.destination = away;
        }
        else
        {
            // Dentro da banda: mantém destino no player para o agent administrar stoppingDistance
            enemy.agent.stoppingDistance = (enemy.attackData.preferMin + enemy.attackData.preferMax) * 0.5f;
            enemy.agent.destination = enemy.player.position;
        }

        // Aplicar "ticks" de dano/efeito conforme attackSpeed (se já não estiver no seu sistema)
        flameTick -= Time.deltaTime;
        if (inBand && flameTick <= 0f)
        {
            // Aqui você poderia invocar um método do controlador da chama para aplicar dano
            // ex.: enemy.flameControl.GetComponent<FlameDamage>().Tick();
            flameTick = Mathf.Max(0.05f, enemy.attackData.attackSpeed);
            enemy.enemyScript.isBurningPlayer = true;
        }

        // Se o player fugiu completamente da janela do ataque, volte ao recovery para reavaliar
        float min = enemy.attackData.attackMinRange;
        float max = enemy.attackData.attackMaxRange;
        if (d < min - 0.5f || d > max + 0.5f)
            stateMachine.ChangeState(enemy.recoveryState);
    }
        

    private void SelectAttackBasedOnDistance()
    {
        float distance = Vector3.Distance(enemy.transform.position, enemy.player.position);

        int wave = Mathf.Max(1, GameController.gameController.currentWave);

        // Exemplo de curva:
        // dano: +10% por wave
        // attackSpeed (intervalo): -5% por wave, com limite mínimo
        float damageMultiplier = 1f + 0.10f * (wave - 1);
        float intervalMultiplier = Mathf.Pow(0.95f, wave - 1); // diminui o intervalo
        float minInterval = 0.15f; // nunca atirar mais rápido que isso



        SelectingAttackData? chosen = null;
        float bestSpan = float.MaxValue;

        foreach (SelectingAttackData attack in enemy.attackList)
        {
            // Faz uma cópia que será ajustada de acordo com a wave
            SelectingAttackData scaledAttack = attack;

            // Escala dano
            scaledAttack.damage = attack.damage * damageMultiplier;

            // Como attackSpeed é intervalo entre ataques, queremos MENOR = mais rápido.
            float scaledInterval = attack.attackSpeed * intervalMultiplier;
            scaledAttack.attackSpeed = Mathf.Max(minInterval, scaledInterval);


            // Verifica se a distância está dentro do alcance desse ataque
            if (distance >= attack.attackMinRange && distance <= attack.attackMaxRange)
            {
                enemy.attackData = attack; // define o ataque atual
                //Debug.Log($"Ataque selecionado: {attack.attackName}");
                
                return;
            }

            float span = 0f;

            if (distance < attack.attackMinRange)
            {
                span = attack.attackMinRange - distance;
            }
            else if (distance > attack.attackMaxRange)
            {
                span = distance - attack.attackMaxRange;
            }

            if (span < bestSpan)
            {
                bestSpan = span;
                chosen = attack;
            }

        }

        //Aqui eu só estou verificando se o Chosen foi definido ou não
        if (chosen.HasValue)
        {
            enemy.attackData = chosen.Value;
        }

    }
}
