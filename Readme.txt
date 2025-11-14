This one just for enemy and player behaviors without grappling

WASD             = Walk around
Mouse            = Look around
Space            = Jump (press twice to double jump!)
F                = Kick (strong, long reach)
Left Click       = Punch (fast, short reach)
Right Click      = Grappling hook (swing or pull enemies)


In Future
- Run and jump around the map
- Double jump to reach high places
- Kick enemies far away
- Punch enemies quickly
- Grapple onto walls to swing like Spider-Man
- Grapple enemies to pull them to you
- Fight basic enemies (easy to kill)
- Fight tank enemies (harder to kill, bigger and slower)


PLAYER (The Capsule):
- Add a Capsule shape (this is you!)
- Add Rigidbody (makes physics work)
- Add Camera as a child (so you can see)
- Add these scripts:
  * PlayerController (for walking and jumping)
  * AttackController (for kick and punch)
  * GrapplingGun (for grappling hook)

ENEMIES (The Cubes):
- Add a Cube shape (this is the enemy)
- Make it a different color (red or gray)
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
- Tank enemies take more hits to kill
- Tank enemies are harder to push
- Use grapple to move around fast
- Enemies will chase you automatically

