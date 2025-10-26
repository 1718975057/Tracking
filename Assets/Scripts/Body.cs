using System;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
public class Body : MonoBehaviour
{
    public class AvatarTree
    {
        public Transform transf;//关节位置
        public AvatarTree child;//子关节
        public AvatarTree parent;//父关节
        public int idx;  // 关节点数据序号
        public Quaternion quaternion;//起始旋转


        public AvatarTree(Transform tf, int idx, Quaternion quaternion, AvatarTree parent = null)
        {
            this.transf = tf;
            this.parent = parent;
            this.idx = idx;
            this.quaternion = quaternion;
        }
        /// <summary>
        /// 获取父子节点方向
        /// </summary>
        /// <returns></returns>
        public Vector3 GetDir()
        {
            if (parent != null)
            {
                return transf.position - parent.transf.position;
            }
            return Vector3.up;
        }
    }
    public Animator anim;//角色动画控制器
    public Transform hip;//臀部
    public Transform spine;//脊椎
    public Transform thorax;//胸部
    public Transform neck;//颈部
    public Transform head;//头部
    public Transform nose;//鼻子
    public Transform lHip;//左臀
    public Transform lKnee;//左膝
    public Transform lFoot;//左脚
    public Transform rHip;//右臀
    public Transform rKnee;//右膝
    public Transform rFoot;//右脚
    public Transform lSld;//左肩
    public Transform lArm;//左膀
    public Transform lEblow;//左肘
    public Transform lWrist;//左手腕
    public Transform rSld;//右肩
    public Transform rArm;//右膀
    public Transform rEblow;//右肘
    public Transform rWrist;//右手腕
    public AvatarTree Hip;
    public AvatarTree LHip;
    private AvatarTree LKnee;
    private AvatarTree LFoot;
    public AvatarTree RHip;
    private AvatarTree RKnee;
    private AvatarTree RFoot;
    private AvatarTree Spine;
    private AvatarTree Thorax;
    private AvatarTree Neck;
    private AvatarTree Head;
    private AvatarTree Nose;
    public AvatarTree LSld;
    private AvatarTree LEblow;
    private AvatarTree LWrist;
    public AvatarTree RSld;
    private AvatarTree REblow;
    private AvatarTree RWrist;
    private AvatarTree LArm;
    private AvatarTree RArm;
    //Thread receiveThread;
    //UdpClient client;
    //public int port = 5056;
    public string data;
    public float[] datas;
    public float lerp;
    private void Start()
    {
        InitAvatar();
        BulidTree();
    }
    private void InitAvatar()
    {
        if (anim != null)
        {
            lHip = anim.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
            lKnee = anim.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
            lFoot = anim.GetBoneTransform(HumanBodyBones.LeftFoot);
            rHip = anim.GetBoneTransform(HumanBodyBones.RightUpperLeg);
            rKnee = anim.GetBoneTransform(HumanBodyBones.RightLowerLeg);
            rFoot = anim.GetBoneTransform(HumanBodyBones.RightFoot);
            spine = anim.GetBoneTransform(HumanBodyBones.Spine);
            thorax = anim.GetBoneTransform(HumanBodyBones.Chest);
            neck = anim.GetBoneTransform(HumanBodyBones.Neck);
            head = anim.GetBoneTransform(HumanBodyBones.Head);
            lSld = anim.GetBoneTransform(HumanBodyBones.LeftShoulder);
            lArm = anim.GetBoneTransform(HumanBodyBones.LeftUpperArm);
            lEblow = anim.GetBoneTransform(HumanBodyBones.LeftLowerArm);
            lWrist = anim.GetBoneTransform(HumanBodyBones.LeftHand);
            rSld = anim.GetBoneTransform(HumanBodyBones.RightShoulder);
            rArm = anim.GetBoneTransform(HumanBodyBones.RightUpperArm);
            rEblow = anim.GetBoneTransform(HumanBodyBones.RightLowerArm);
            rWrist = anim.GetBoneTransform(HumanBodyBones.RightHand);
        }
    }//匹配关节点
    private void BulidTree()
    {
        Hip = new AvatarTree(hip, -1, hip.rotation);// -1
        Spine = Hip.child = new AvatarTree(spine, -2, spine.rotation, Hip);// -2
        Thorax = Spine.child = new AvatarTree(thorax, -3, thorax.rotation, Spine);// -3
        Neck = Thorax.child = new AvatarTree(neck, -4, neck.rotation, Thorax);// -4
        Head = Neck.child = new AvatarTree(head, -5, head.rotation, Neck);// -5
        Nose = Head.child = new AvatarTree(nose, 0, nose.rotation, Head);
        LHip = new AvatarTree(lHip, 23, lHip.rotation);
        LKnee = LHip.child = new AvatarTree(lKnee, 25, lKnee.rotation, LHip);
        LFoot = LKnee.child = new AvatarTree(lFoot, 29, lFoot.rotation, LKnee);
        RHip = new AvatarTree(rHip, 24, rHip.rotation);
        RKnee = RHip.child = new AvatarTree(rKnee, 26, rHip.rotation, RHip);
        RFoot = RKnee.child = new AvatarTree(rFoot, 30, rFoot.rotation, RKnee);
        LSld = new AvatarTree(lSld, -6, lSld.rotation);// -6
        LArm = LSld.child = new AvatarTree(lArm, 11, lArm.rotation, LSld);
        LEblow = LArm.child = new AvatarTree(lEblow, 13, lEblow.rotation, LArm);
        LWrist = LEblow.child = new AvatarTree(lWrist, 15, lWrist.rotation, LEblow);
        RSld = new AvatarTree(rSld, -7, rSld.rotation); // -7
        RArm = RSld.child = new AvatarTree(rArm, 12, rArm.rotation, RSld);
        REblow = RArm.child = new AvatarTree(rEblow, 14, rEblow.rotation, RArm);
        RWrist = REblow.child = new AvatarTree(rWrist, 16, rWrist.rotation, REblow);
    }//匹配关节父子节点
    void Update()
    {
        lerp += Time.deltaTime;
        if (lerp >= 1.0f)
        {
            lerp = 0;
        }
        if (data != "")
        {
            datas = Array.ConvertAll(data.Remove(data.Length - 1).Split(','), float.Parse);
            UpdateTree(Hip, lerp);
            UpdateTree(RArm, lerp);
            UpdateTree(LArm, lerp);
            //UpdateTree(LHip, lerp);
            //UpdateTree(RHip, lerp);
        }
    }
    private Vector3 GetData(int idx)
    {
        float x, y, z;
        //因为Mediapipe给的标准关键点跟人物的关节点不同（比如上体Mediapipe给的是左肩右肩左腰右腰的点，但实际上需要的是脊椎的关节点）
        if (idx == -1)
        {
            x = (datas[23 * 3] + datas[24 * 3]) / 2;
            y = (datas[23 * 3 + 1] + datas[24 * 3 + 1]) / 2;
            z = (datas[23 * 3 + 2] + datas[24 * 3 + 2]) / 2;
        }
        else if (idx == -2)
        {
            x = ((datas[11 * 3] + datas[12 * 3]) / 2 + (datas[23 * 3] + datas[24 * 3]) / 2) / 2;
            y = ((datas[11 * 3 + 1] + datas[12 * 3 + 1]) / 2 + (datas[23 * 3 + 1] + datas[24 * 3 + 1]) / 2) / 2;
            z = ((datas[11 * 3 + 2] + datas[12 * 3 + 2]) / 2 + (datas[23 * 3 + 2] + datas[24 * 3 + 2]) / 2) / 2;
        }
        else if (idx == -3)
        {
            x = (datas[11 * 3] + datas[12 * 3]) / 2;
            y = (datas[11 * 3 + 1] + datas[12 * 3 + 1]) / 2;
            z = (datas[11 * 3 + 2] + datas[12 * 3 + 2]) / 2;
        }
        else if (idx == -4)
        {
            x = ((datas[8 * 3] + datas[7 * 3]) / 2 + (datas[11 * 3] + datas[12 * 3]) / 2) / 2;
            y = ((datas[8 * 3 + 1] + datas[7 * 3 + 1]) / 2 + (datas[11 * 3 + 1] + datas[12 * 3 + 1]) / 2) / 2;
            z = ((datas[8 * 3 + 2] + datas[7 * 3 + 2]) / 2 + (datas[11 * 3 + 2] + datas[12 * 3 + 2]) / 2) / 2;
        }
        else if (idx == -5)
        {
            x = (datas[8 * 3] + datas[7 * 3]) / 2;
            y = (datas[8 * 3 + 1] + datas[7 * 3 + 1]) / 2;
            z = (datas[8 * 3 + 2] + datas[7 * 3 + 2]) / 2;
        }
        else if (idx == -6)
        {
            x = (datas[11 * 3] * 3 + datas[12 * 3]) / 4;
            y = (datas[11 * 3 + 1] * 3 + datas[12 * 3 + 1]) / 4;
            z = (datas[11 * 3 + 2] * 3 + datas[12 * 3 + 2]) / 4;
        }
        else if (idx == -7)
        {
            x = (datas[11 * 3] + datas[12 * 3] * 3) / 4;
            y = (datas[11 * 3 + 1] + datas[12 * 3 + 1] * 3) / 4;
            z = (datas[11 * 3 + 2] + datas[12 * 3 + 2] * 3) / 4;
        }
        else
        {
            x = datas[idx * 3];
            y = datas[idx * 3 + 1];
            z = datas[idx * 3 + 2];
        }
        return new Vector3(-x, y, -z);
    }
    private void UpdateTree(AvatarTree tree, float lerp)
    {
        if (tree.parent != null)
        {
            UpdateBone(tree, lerp);
        }
        if (tree.child != null)
        {
            UpdateTree(tree.child, lerp);
        }
    }//遍历关节
    private void UpdateBone(AvatarTree tree, float lerp)
    {
        var dir1 = tree.GetDir();
        var dir2 = GetData(tree.parent.idx) - GetData(tree.idx);
        Quaternion rot = Quaternion.FromToRotation(dir1, dir2);
        Quaternion rot1 = tree.parent.transf.rotation;
        tree.parent.transf.rotation = Quaternion.Lerp(rot1, rot * rot1, lerp);
    }//计算骨骼旋转
}
