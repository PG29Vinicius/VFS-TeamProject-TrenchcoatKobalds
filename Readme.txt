This one just for enemy and player behaviors without grappling

WASD             = Walk around
Mouse            = Look around
Space            = Jump (press twice to double jump!)
F                = Kick (strong, long reach)
G       = Punch (fast, short reach)



In the Future: 
- Run and jump around the map
- Double jump to reach high places
- Kick enemies far away
- Punch enemies quickly
- Fight basic enemies (easy to kill)


PLAYER (The Capsule):
- Add a Capsule shape (this is you!)
- Add Rigidbody (makes physics work)
- Add Camera as a child (so you can see)
- Add these scripts:
  * PlayerController (for walking and jumping)
  * AttackController (for kick and punch)

ENEMIES (The Cubes):
- Add a Cube shape (this is the enemy)
- Add Rigidbody (so it can get knocked back)
- Add these scripts:
  * Enemy (for health and chasing you)
  * Knockback (so attacks push it away)
- Set Tag to "Enemy" 


PlayerController    = Makes you walk, jump, and look around
AttackController    = Makes kick and punch work
GrapplingGun       = Makes the grappling hook work
Enemy              = Makes enemies chase you and take damage
Knockback          = Makes enemies fly back when you hit them


- Kick is stronger but slower
- Punch is weaker but faster
- Use grapple to move around fast
- Enemies will chase you automatically

