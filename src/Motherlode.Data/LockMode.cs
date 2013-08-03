namespace Motherlode.Data
{
    public enum LockMode
    {
        /// <summary>
        ///     No lock required.
        /// </summary>
        /// <remarks>
        ///     If an object is requested with this lock mode, a <c>Read</c> lock
        ///     might be obtained if necessary.
        /// </remarks>
        None,

        /// <summary>
        ///     A shared lock.
        /// </summary>
        /// <remarks>
        ///     Objects are loaded in <c>Read</c> mode by default
        /// </remarks>
        Read,

        /// <summary>
        ///     An upgrade lock.
        /// </summary>
        /// <remarks>
        ///     Objects loaded in this lock mode are materialized using an
        ///     SQL <c>SELECT ... FOR UPDATE</c>
        /// </remarks>
        Upgrade,

        /// <summary>
        ///     Attempt to obtain an upgrade lock, using an Oracle-style
        ///     <c>SELECT ... FOR UPGRADE NOWAIT</c>.
        /// </summary>
        /// <remarks>
        ///     The semantics of this lock mode, once obtained, are the same as <c>Upgrade</c>
        /// </remarks>
        UpgradeNoWait,

        /// <summary>
        ///     A <c>Write</c> lock is obtained when an object is updated or inserted.
        /// </summary>
        /// <remarks>
        ///     This is not a valid mode for <c>Load()</c> or <c>Lock()</c>.
        /// </remarks>
        Write,

        /// <summary>
        ///     Similar to <see cref="Upgrade" /> except that, for versioned entities,
        ///     it results in a forced version increment.
        /// </summary>
        Force
    }
}
