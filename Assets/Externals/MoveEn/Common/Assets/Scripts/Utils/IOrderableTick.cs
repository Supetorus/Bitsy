
namespace moveen.descs {
    public interface IOrderableTick {
        int getOrder();
        void tick(float dt);
        void fixedTick(float dt);
        bool getParticipateInTick();
        bool getParticipateInFixedTick();
        void setIsBroken(bool value);
        bool getIsBroken();
    }
}