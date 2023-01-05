using System;
using System.Collections.Generic;
using moveen.descs;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace moveen.utils {
    /// <summary>
    /// In computational animation sometimes very important to execute scripts in a very specific order. For example, the position of the leg should be known strictly before the position of the knee is calculated.
    /// <para/>
    /// This utility class implements strictly ordered execution of IOrderableTick.tick.
    /// <para/>
    /// In order to do that, OrderedMonoBehaviour calls OrderedTick' methods in the respective Update, LateUpdate, and Fixed Update methods. OrderedTick uses those calls to execute ordered calls to tick and fixedTick in all registered IOrderableTick (registration for OrderedMonoBehaviour occurs automatically).
    /// <para/>
    /// OrderedTick calls IOrderableTick tick and fixedTick exactly once per Unity cycle and in the order defined by IOrderableTick.getOrder().
    /// The ordering between objects with the same ordering value is not specified.
    /// First IOrderableTick.tick is always called before first IOrderableTick.fixedTick (for consistency of behavior).
    /// <para/>
    /// </summary>
    public static class OrderedTick {
        /// <summary>
        /// Should the tick(dt) be called in LateUpdate section (true). In this case, all ordered ticks will be called strictly after all Update calls, somewhere between LateUpdate calls.
        /// <para/>
        /// If set to false, all ordered ticks will be called strictly before all LateUpdate calls, somewhere between Update calls.
        /// </summary>
        public static bool tickInLateUpdate = true;
        
        //All IOrderableTick that will be called in the next tick
        private static List<IOrderableTick> all = new List<IOrderableTick>();
        //Newly added IOrderableTick. Note, that no newly added object will tick in the same tick it has been added. It only will be added and sorted in the next tick.
        private static List<IOrderableTick> toAdd = new List<IOrderableTick>();
        //Shows if ticks should be called (true) in this cycle (Update, LateUpdate), or it was already called (false).
        //Note, ticks are called when any (first) Update/LateUpdate of OrderedMonoBehaviour is called.
        private static bool readyForTick = !tickInLateUpdate;
        //Shows if fixedTicks should be called (true) in this cycle (FixedUpdate), or it was already called (false).
        //Note, fixedTicks are called when any (first) FixedUpdate of OrderedMonoBehaviour is called.
        private static bool readyForFixedTick;
        //Is all is sorted (true), or is sorting (on the next tick) is needed (false).
        //Set false on adding a new object or when setUnsorted is explicitly called.
        private static bool isSorted;

        // public static CounterStacksCollection paramHistory = new CounterStacksCollection(100);//TODO some global switch
        // public static List<HistoryInfoBean> historyBeans = new List<HistoryInfoBean>() {
        //     new HistoryInfoBean("fixedUpdate", Color.red),
        //     new HistoryInfoBean("lateUpdate", Color.green),
        // };

        /// <summary>
        /// Sets unsorted state, which leads to the sorting of all objects on the next tick (eg when ordering value has changed).
        /// </summary>
        public static void setUnsorted() {
            isSorted = false;
        }

        /// <summary>
        /// Adds a new component. Actual add and sorting will be implemented inside (next) tick.
        /// </summary>
        public static void addComponent(IOrderableTick co) {
            toAdd.Add(co);
            isSorted = false;
        }

        //TODO delete better, because of possible skip
        //TO DO maybe require delete only outside of tick?
        //  and check "is enabled"
        /// <summary>
        /// Immediately removes a component.
        /// If the call happens inside a tick, the component (of course) could already be ticked. And it will not tick if it hasn't yet.
        /// </summary>
        public static void deleteComponent(IOrderableTick co) {
            all.Remove(co);
        }

        //TO DO fix broken order if called inside a tick
        //      or just require to be called outside of any tick
        //TO DO remember index inside a component?
        // public static void deleteComponentFast(OrderedMonoBehaviour co) {
        //     int i = all.IndexOf(co);
        //     if (i == -1) {
        //         Debug.LogError("index of MonoBehaviour == -1");
        //     } else {
        //         all[i] = all[all.Count - 1];
        //         all.RemoveAt(all.Count - 1);
        //         isSorted = false;
        //     }
        // }

        // Is called by Unity FixedUpdate (as many times as there are Ordered MonoBehaviour).
        // Calls fixedTick of each IOrderableTick (only once per Unity cycle).
        public static void onUnityFixedUpdate(float dt) {
            // paramHistory.next();
            // paramHistory.setValue("fixedUpdate", -1);
            if (!readyForFixedTick) return;
            readyForFixedTick = false;
            for (int i = 0; i < all.Count; i++) {
                IOrderableTick tickable = all[i];
                if (!tickable.getIsBroken() && tickable.getParticipateInFixedTick()) {
                    try
                    {
                        var behaviour = tickable as OrderedMonoBehaviour;
                        if (ReferenceEquals(behaviour, null) || behaviour)//because editor can kill objects without any event. Is there performance issues in this check?
                            tickable.fixedTick(dt);
                    } catch (Exception e) {
                        tickable.setIsBroken(true);
                        Debug.LogError("error at i " + i);
                        Debug.LogError(e);
                    }
                }
            }
        }

        // Is called after all Unity FixedUpdate already called (as many times as there are Ordered MonoBehaviour).
        // Helps onUnityFixedUpdate on fixedTick calling.
        public static void onUnityLateFixedUpdate() {
            readyForFixedTick = true;
        }

        // Is called by Unity Update (as many times as there are Ordered MonoBehaviour).
        // Helps call tick only once per cycle.
        public static void onUnityUpdate() {
            if (tickInLateUpdate) readyForTick = true;
            else tick(Time.deltaTime);
        }

        // Is called by Unity LateUpdate (as many times as there are Ordered MonoBehaviour).
        // Helps call tick only once per cycle.
        public static void onUnityLateUpdate() {
            if (tickInLateUpdate) tick(Time.deltaTime);
            else readyForTick = true;
        }

        // Is called by Unity Update/LateUpdate (as many times as there are Ordered MonoBehaviour).
        // Calls tick of each IOrderableTick (only once per Unity cycle).
        private static void tick(float dt) {
            // paramHistory.setValue("lateUpdate", 1);
            if (!readyForTick) return;
            readyForTick = false;
            sort();
            for (int i = 0; i < all.Count; i++) {
                IOrderableTick tickable = all[i];
                if (!tickable.getIsBroken() && tickable.getParticipateInTick()) {
                    try
                    {
                        var behaviour = tickable as OrderedMonoBehaviour;
                        if (ReferenceEquals(behaviour, null) || behaviour)//because editor can kill objects without any event. Is there performance issues in this check?
                            tickable.tick(dt);
                    } catch (Exception e) {
                        tickable.setIsBroken(true);
                        Debug.LogError("error at i " + i);
                        Debug.LogError(e);
                    }
                }
            }
        }

        // Is called once per tick (not fixedTick!).
        // So it is guaranteed that the first tick will always be called before the first fixedTick.
        private static void sort() {
            if (isSorted) return;
            for (int i = 0; i < toAdd.Count; i++) all.Add(toAdd[i]);
            toAdd.Clear();
            all.Sort(compareOrderable);
            isSorted = true;
        }

        private static int compareOrderable(IOrderableTick a, IOrderableTick b) {
            return a.getOrder() - b.getOrder();
        }

        public static void forceTick(float dt) {
            for (int i = 0; i < all.Count; i++) all[i].tick(dt);
        }

    }
}