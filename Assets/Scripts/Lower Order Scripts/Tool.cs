using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class Tool : MonoBehaviour
{
    [SerializeField]
    private Transform addTenPos;

    [SerializeField]
    private float speed;

    [SerializeField]
    private SpriteRenderer addOneSprite;

    [SerializeField]
    SpriteLibraryAsset[] tool_variants;

    [SerializeField]
    Sprite[] hit_variant;

    private SpriteLibrary tool_sprites;
    private Animator statusAnimator;
    private AnimationClip[] clips;
    
    void Awake()
    {
        statusAnimator = GetComponentInChildren<Animator>();
        clips = statusAnimator.runtimeAnimatorController.animationClips;
        Debug.Log(clips.Length);

        tool_sprites = GetComponentInChildren<SpriteLibrary>();
        //events = clips[1].events;

        //Debug.Log("Event time: " + events[0].time);
    }

    public void SetToolLook(int variant)
    {
        if (tool_variants.Count() > variant)
        {
            tool_sprites.spriteLibraryAsset = tool_variants[variant];
            Debug.Log("Setting Hit Icon Design");
            addOneSprite.sprite = hit_variant[variant];
        }
    }

    public IEnumerator TriggerToolAnimation(PartTile fastener)
    {
        GetComponentInChildren<ToolEvent>().SetCurrentFastener(fastener);
        GetComponent<Collider2D>().enabled = false;
        statusAnimator.SetTrigger("IsUsed");
        yield return new WaitForSeconds(1f);
        GetComponent<Collider2D>().enabled = true;
    }

    public void SetHeightValue(float value)
    {
        GetComponentInChildren<ToolEvent>().SetHeightToSet(value);
    }

    public Vector3 GetAddTenPos()
    {
        return Camera.main.WorldToScreenPoint(addTenPos.position);
    }

    public IEnumerator TriggerToolChange(float value)
    {
        Vector2 position = new Vector2(0, value);

        Debug.Log(position);

        Debug.Log(transform.localPosition);

        while (Vector2.Distance(transform.localPosition, position) > 0.001f)
        {
            transform.localPosition = Vector2.MoveTowards(transform.localPosition, position, speed * Time.deltaTime);
            yield return null;
        }
    }
}
